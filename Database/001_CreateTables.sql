USE master;
GO


IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TestingSystem')
BEGIN
    CREATE DATABASE TestingSystem;
END
GO

USE TestingSystem;
GO

-- Таблица ролей
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200)
);
GO

-- Таблица пользователей
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Login NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100),
    RoleId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

-- Таблица дисциплин
CREATE TABLE Disciplines (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    TeacherId INT,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Disciplines_Teacher FOREIGN KEY (TeacherId) REFERENCES Users(Id)
);
GO

-- Таблица связи пользователей и дисциплин (студенты на дисциплинах)
CREATE TABLE UserDisciplines (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    DisciplineId INT NOT NULL,
    AssignedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_UserDisciplines_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_UserDisciplines_Discipline FOREIGN KEY (DisciplineId) REFERENCES Disciplines(Id),
    CONSTRAINT UQ_UserDiscipline UNIQUE (UserId, DisciplineId)
);
GO

-- Таблица тестов
CREATE TABLE Tests (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DisciplineId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    TimeLimitMinutes INT,
    PassingScore INT NOT NULL DEFAULT 50,
    IsPublished BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Tests_Discipline FOREIGN KEY (DisciplineId) REFERENCES Disciplines(Id)
);
GO

-- Таблица вопросов
CREATE TABLE Questions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TestId INT NOT NULL,
    Text NVARCHAR(1000) NOT NULL,
    QuestionType INT NOT NULL, -- 0 = один ответ, 1 = несколько, 2 = текст
    Points INT NOT NULL DEFAULT 1,
    OrderNumber INT NOT NULL,
    CONSTRAINT FK_Questions_Test FOREIGN KEY (TestId) REFERENCES Tests(Id)
);
GO

-- Таблица ответов
CREATE TABLE Answers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    QuestionId INT NOT NULL,
    Text NVARCHAR(500) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    OrderNumber INT NOT NULL,
    CONSTRAINT FK_Answers_Question FOREIGN KEY (QuestionId) REFERENCES Questions(Id)
);
GO

-- Таблица результатов тестов
CREATE TABLE TestResults (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    TestId INT NOT NULL,
    Score INT NOT NULL DEFAULT 0,
    TotalPoints INT NOT NULL DEFAULT 0,
    Percentage INT NOT NULL DEFAULT 0,
    IsPassed BIT NOT NULL DEFAULT 0,
    StartedAt DATETIME2,
    CompletedAt DATETIME2,
    CONSTRAINT FK_TestResults_User FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT FK_TestResults_Test FOREIGN KEY (TestId) REFERENCES Tests(Id)
);
GO

-- Таблица ответов пользователя
CREATE TABLE UserAnswers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TestResultId INT NOT NULL,
    QuestionId INT NOT NULL,
    SelectedAnswerId INT,
    TextAnswer NVARCHAR(1000),
    IsCorrect BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_UserAnswers_Result FOREIGN KEY (TestResultId) REFERENCES TestResults(Id),
    CONSTRAINT FK_UserAnswers_Question FOREIGN KEY (QuestionId) REFERENCES Questions(Id),
    CONSTRAINT FK_UserAnswers_Answer FOREIGN KEY (SelectedAnswerId) REFERENCES Answers(Id)
);
GO

-- Индексы для производительности
CREATE INDEX IX_Users_Login ON Users(Login);
CREATE INDEX IX_Users_RoleId ON Users(RoleId);
CREATE INDEX IX_Disciplines_TeacherId ON Disciplines(TeacherId);
CREATE INDEX IX_UserDisciplines_UserId ON UserDisciplines(UserId);
CREATE INDEX IX_UserDisciplines_DisciplineId ON UserDisciplines(DisciplineId);
CREATE INDEX IX_Tests_DisciplineId ON Tests(DisciplineId);
CREATE INDEX IX_Questions_TestId ON Questions(TestId);
CREATE INDEX IX_Answers_QuestionId ON Answers(QuestionId);
CREATE INDEX IX_TestResults_UserId ON TestResults(UserId);
CREATE INDEX IX_TestResults_TestId ON TestResults(TestId);
CREATE INDEX IX_UserAnswers_TestResultId ON UserAnswers(TestResultId);
CREATE INDEX IX_UserAnswers_QuestionId ON UserAnswers(QuestionId);
GO

PRINT 'Таблицы созданны';
GO