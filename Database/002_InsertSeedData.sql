USE TestingSystem;
GO

-- 1. Роли
INSERT INTO Roles (Name, Description) VALUES
('Admin', 'Администратор системы - полный доступ'),
('Teacher', 'Преподаватель - создание и управление тестами'),
('Student', 'Студент - прохождение тестов');

PRINT 'Роли добавлены!';
GO

-- 2. Пользователи (admin + учитель + студент)
INSERT INTO Users (Login, PasswordHash, FirstName, LastName, Email, RoleId, CreatedAt)
VALUES 
('admin', '$2a$12$rs5wMYlLuzQc8qCDwc3AIuScqoKldizye9vh1u3iMmZ6F0Elq9Cxe', 'Админ', 'Системы', 'admin@testingsystem.com', 1, GETDATE()),
('teacher', '$2a$12$rs5wMYlLuzQc8qCDwc3AIuScqoKldizye9vh1u3iMmZ6F0Elq9Cxe', 'Иван', 'Петров', 'teacher@testingsystem.com', 2, GETDATE()),
('student', '$2a$12$rs5wMYlLuzQc8qCDwc3AIuScqoKldizye9vh1u3iMmZ6F0Elq9Cxe', 'Пётр', 'Сидоров', 'student@testingsystem.com', 3, GETDATE());

PRINT 'Пользователи добавлены!';
GO

-- 3. Дисциплины (TeacherId = 2 — это teacher)
INSERT INTO Disciplines (Name, Description, TeacherId, CreatedAt)
VALUES 
('C# Программирование', 'Основы программирования на языке C# и .NET', 2, GETDATE()),
('Базы данных', 'Проектирование и управление базами данных, SQL', 2, GETDATE()),
('Веб-разработка', 'Создание веб-приложений на ASP.NET Core', 2, GETDATE());

PRINT 'Дисциплины добавлены!';
GO

-- 4. Тесты
INSERT INTO Tests (DisciplineId, Title, Description, TimeLimitMinutes, PassingScore, IsPublished, CreatedAt)
VALUES 
(1, 'Основы C#', 'Проверка знаний основ программирования на C#', 30, 50, 1, GETDATE()),
(2, 'SQL запросы', 'Проверка знаний SQL: SELECT, INSERT, UPDATE, DELETE', 20, 60, 1, GETDATE());

PRINT 'Тесты добавлены!';
GO

-- 5. Вопросы для теста "Основы C#" (TestId = 1)
DECLARE @TestId1 INT = 1;

INSERT INTO Questions (TestId, Text, QuestionType, Points, OrderNumber)
VALUES (@TestId1, 'Какой тип данных используется для хранения целых чисел?', 0, 1, 1);
DECLARE @Q1 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect, OrderNumber) VALUES
(@Q1, 'int', 1, 1),
(@Q1, 'string', 0, 2),
(@Q1, 'bool', 0, 3),
(@Q1, 'double', 0, 4);

INSERT INTO Questions (TestId, Text, QuestionType, Points, OrderNumber)
VALUES (@TestId1, 'Что выведет Console.WriteLine("Hello" + " " + "World")?', 0, 1, 2);
DECLARE @Q2 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect, OrderNumber) VALUES
(@Q2, 'Hello World', 1, 1),
(@Q2, 'HelloWorld', 0, 2),
(@Q2, 'Hello+World', 0, 3);

INSERT INTO Questions (TestId, Text, QuestionType, Points, OrderNumber)
VALUES (@TestId1, 'Какие из следующих являются типами значений? (выберите все правильные)', 1, 2, 3);
DECLARE @Q3 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect, OrderNumber) VALUES
(@Q3, 'int', 1, 1),
(@Q3, 'struct', 1, 2),
(@Q3, 'class', 0, 3),
(@Q3, 'string', 0, 4);

-- 6. Вопросы для теста "SQL запросы" (TestId = 2)
DECLARE @TestId2 INT = 2;

INSERT INTO Questions (TestId, Text, QuestionType, Points, OrderNumber)
VALUES (@TestId2, 'Какая команда используется для выборки данных из базы?', 0, 1, 1);
DECLARE @Q4 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect, OrderNumber) VALUES
(@Q4, 'SELECT', 1, 1),
(@Q4, 'GET', 0, 2),
(@Q4, 'FETCH', 0, 3),
(@Q4, 'RETRIEVE', 0, 4);

INSERT INTO Questions (TestId, Text, QuestionType, Points, OrderNumber)
VALUES (@TestId2, 'Что означает WHERE в SQL запросе?', 0, 1, 2);
DECLARE @Q5 INT = SCOPE_IDENTITY();

INSERT INTO Answers (QuestionId, Text, IsCorrect, OrderNumber) VALUES
(@Q5, 'Условие фильтрации', 1, 1),
(@Q5, 'Сортировка', 0, 2),
(@Q5, 'Группировка', 0, 3);

PRINT 'Seed data полностью добавлена!';
GO