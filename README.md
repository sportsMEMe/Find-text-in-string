# Find-text-in-string-uploading-SQL

This application is designed to...
1.	Loading data from a text file into a database. 
The application loads into the database from the input text file (which may contain text in English and Russian, UTF-8 encoding) all words that meet the following criteria:
     (a) The length of the word is not less than 3 and not more than 20 characters;
     (b) the word is mentioned at least 4 times in the current input file.
2.	Loading can be performed multiple times. The application does not delete existing data from the database, but augments it.
3.	In the database for each saved word, the number of its mentions summarized for all uploaded files should be stored.
4. the text file fed to the input is UTF-8. The file may contain any letters (Latin and Cyrillic) and spaces. The file can contain more than one line. The file can be up to 100 MB in size.
5. Automatic creation and initialization of the database.
6. MS SQL Server database

============================================

Данное приложение предназначено для...
1.	Загрузка данных из текстового файла в базу данных. 
Приложение загружает в базу данных из входного текстового файла (который может содержать текст на английском и русском языках, в кодировке UTF-8) все слова, удовлетворяющие следующим критериям:
 a)	длина слова не менее 3 и не более 20 символов;
 b)	слово упоминается в текущем входном файле не менее 4-ёх раз.
2.	Загрузка может выполняться многократно. При этом приложение не удаляет существующие данные из базы данных, а дополнять их.
3.	В базе данных для каждого сохранённого слова должно храниться количество его упоминаний, суммарное для всех загруженных файлов.
4.	Текстовый файл, подаваемый на вход,  UTF-8. Файл может содержать любые буквы (латиница и кириллица) и пробелы. Файл может содержать более одной строки. Файл может иметь размер до 100 МБ.
5. Автоматическое создание и инициализация базы данных.
6. База данных MS SQL Server
