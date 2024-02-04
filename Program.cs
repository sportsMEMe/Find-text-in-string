using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите имя сервера:");
        string serverName = Console.ReadLine();

        Console.WriteLine("Введите имя базы данных:");
        string dbName = Console.ReadLine();

        // Подключение к серверу (без базы данных)
        string serverConnectionString = $"Data Source={serverName};Integrated Security=True";

        // Создание базы данных, если она отсутствует
        CreateDatabaseIfNotExists(serverConnectionString, dbName);

        string connectionString = $"Data Source={serverName};Initial Catalog={dbName};Integrated Security=True";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            // Создание таблицы, если она отсутствует
            connection.Open();
            InitializeDatabase(connection);

            // Загрузка данных из всех файлов
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            LoadDataFromFolder(folderPath, connection);
        }
    }

    static void CreateDatabaseIfNotExists(string serverConnectionString, string dbName)
    {
        string createDatabaseQuery = $@"
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}')
            BEGIN
                CREATE DATABASE {dbName};
            END";

        using (SqlConnection serverConnection = new SqlConnection(serverConnectionString))
        {
            serverConnection.Open();

            using (SqlCommand createDbCommand = new SqlCommand(createDatabaseQuery, serverConnection))
            {
                createDbCommand.ExecuteNonQuery();
            }
        }
    }

    static void InitializeDatabase(SqlConnection connection)
    {
        string createTableQuery = @"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tabword')
        BEGIN
            CREATE TABLE tabword 
            (
                id INT IDENTITY(1,1) NOT NULL, 
                word VARCHAR (20),
                volume INT,
                PRIMARY KEY (id)
            )
        END";

        using (SqlCommand command = new SqlCommand(createTableQuery, connection))
        {
            command.ExecuteNonQuery();
        }
    }

    static void LoadDataFromFolder(string folderPath, SqlConnection connection)
    {
        Dictionary<string, int> wordOccurrences = new Dictionary<string, int>();

        string[] files = Directory.GetFiles(folderPath, "*.txt");

        foreach (var filePath in files)
        {
            string text = File.ReadAllText(filePath);
            
            // Регулярное выражение - для поиска слов ТОЛЬКО английские и русские символы, не учитывая знаки
            Regex regex = new Regex(@"\b(?:[а-яА-Яa-zA-Z]{3,20}\d*)(?:[.,;:!?\-']*)\b");
            // Сопоставление слов с регулярным выражением
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                string word = match.Value.ToLower(); // Приводим слова к нижнему регистру
               
                // Проверка длины слова
                if (word.Length >= 3 && word.Length <= 20)
                {
                    // Проверка слова в словаре
                    wordOccurrences[word] = wordOccurrences.ContainsKey(word) ? wordOccurrences[word] + 1 : 1;
                }
            }
        }

        // Фильтруем слова по условиям и добавляем в базу данных
        foreach (var entry in wordOccurrences.Where(entry => entry.Value >= 4))
        {
            UpdateDatabase(entry.Key, entry.Value, connection);
            Console.WriteLine($"Слово '{entry.Key}' добавлено {entry.Value} {GetWordCountString(entry.Value)}");
        }

        Console.ReadLine(); //пауза
    }

    static void UpdateDatabase(string word, int volume, SqlConnection connection)
    {
        string query = @"
            MERGE tabword 
            USING (VALUES (@word)) AS source (word)
            ON tabword.word = source.word
            WHEN MATCHED THEN
                UPDATE SET tabword.volume = tabword.volume + @volume
            WHEN NOT MATCHED THEN
                INSERT (word, volume) VALUES (source.word, @volume)
            OUTPUT INSERTED.id;";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@word", word);
            command.Parameters.AddWithValue("@volume", volume);

            command.ExecuteScalar();
        }
    }

    static string GetWordCountString(int count)
    {
        string[] forms = { "раз", "раза", "раз" };
        int remainder10 = count % 10;
        int remainder100 = count % 100;

        if (remainder10 == 1 && remainder100 != 11)
        {
            return forms[0];
        }
        else if (remainder10 >= 2 && remainder10 <= 4 && (remainder100 < 10 || remainder100 >= 20))
        {
            return forms[1];
        }
        else
        {
            return forms[2];
        }
    }
}