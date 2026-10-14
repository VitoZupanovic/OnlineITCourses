CREATE TABLE dbo.CourseGroup (
    Id INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,

    CONSTRAINT PK_CourseGroup PRIMARY KEY (Id),
    CONSTRAINT UQ_CourseGroup_Name UNIQUE ([Name])
);
go
CREATE TABLE dbo.Course (
    Id INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(1000) NOT NULL,
    DurationHours INT NOT NULL,
    [Level] NVARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    CourseGroupId INT NOT NULL,

    CONSTRAINT PK_Course PRIMARY KEY (Id),
    CONSTRAINT UQ_Course_Name UNIQUE ([Name]),
    CONSTRAINT FK_Course_CourseGroup FOREIGN KEY (CourseGroupId)
        REFERENCES dbo.CourseGroup(Id),
    CONSTRAINT CK_Course_DurationHours CHECK (DurationHours > 0),
    CONSTRAINT CK_Course_Price CHECK (Price >= 0)
);
go
CREATE TABLE dbo.Instructor (
    Id INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(150) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Biography NVARCHAR(1000) NULL,

    CONSTRAINT PK_Instructor PRIMARY KEY (Id),
    CONSTRAINT UQ_Instructor_Email UNIQUE (Email)
);
go
CREATE TABLE dbo.CourseInstructor (
    Id INT IDENTITY(1,1) NOT NULL,
    CourseId INT NOT NULL,
    InstructorId INT NOT NULL,

    CONSTRAINT PK_CourseInstructor PRIMARY KEY (Id),
    CONSTRAINT FK_CourseInstructor_Course FOREIGN KEY (CourseId)
        REFERENCES dbo.Course(Id),
    CONSTRAINT FK_CourseInstructor_Instructor FOREIGN KEY (InstructorId)
        REFERENCES dbo.Instructor(Id),
    CONSTRAINT UQ_CourseInstructor UNIQUE (CourseId, InstructorId)
);
go
CREATE TABLE dbo.[Role] (
    Id INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_Role PRIMARY KEY (Id),
    CONSTRAINT UQ_Role_Name UNIQUE ([Name])
);
go
CREATE TABLE dbo.[User] (
    Id INT IDENTITY(1,1) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    PwdSalt NVARCHAR(256) NOT NULL,
    PwdHash NVARCHAR(256) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(50) NULL,
    RoleId INT NOT NULL,

    CONSTRAINT PK_User PRIMARY KEY (Id),
    CONSTRAINT UQ_User_Username UNIQUE (Username),
    CONSTRAINT UQ_User_Email UNIQUE (Email),
    CONSTRAINT FK_User_Role FOREIGN KEY (RoleId)
        REFERENCES dbo.[Role](Id)
);
go
CREATE TABLE dbo.[Application] (
    Id INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(150) NOT NULL,
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    AppliedAt DATETIME2 NOT NULL,
    [Status] NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_Application PRIMARY KEY (Id),
    CONSTRAINT FK_Application_User FOREIGN KEY (UserId)
        REFERENCES dbo.[User](Id),
    CONSTRAINT FK_Application_Course FOREIGN KEY (CourseId)
        REFERENCES dbo.Course(Id),
    CONSTRAINT UQ_Application_User_Course UNIQUE (UserId, CourseId)
);
go
CREATE TABLE dbo.[Log] (
    Id INT IDENTITY(1,1) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL,
    [Level] INT NOT NULL,
    [Message] NVARCHAR(1024) NOT NULL,
    ErrorText NVARCHAR(MAX) NULL,

    CONSTRAINT PK_Log PRIMARY KEY (Id),
    CONSTRAINT CK_Log_Level CHECK ([Level] BETWEEN 1 AND 5)
);
go
INSERT INTO dbo.CourseGroup ([Name], [Description])
VALUES
('Security', 'Courses related to cybersecurity and information security.'),
('Programming Languages', 'Courses related to programming languages.'),
('Machine Learning', 'Courses related to artificial intelligence and machine learning.'),
('Web Development', 'Courses related to frontend and backend web development.');
go
INSERT INTO dbo.Course ([Name], [Description], DurationHours, [Level], Price, CourseGroupId)
VALUES
('C# Fundamentals', 'Learn the basics of C# programming language.', 20, 'Beginner', 49.99, 2),
('ASP.NET Core Web API', 'Learn how to build RESTful APIs using ASP.NET Core.', 30, 'Intermediate', 79.99, 4),
('Introduction to Cybersecurity', 'Learn basic cybersecurity concepts and common threats.', 25, 'Beginner', 59.99, 1),
('Machine Learning Basics', 'Learn the fundamentals of machine learning.', 35, 'Beginner', 89.99, 3);
go
INSERT INTO dbo.Instructor ([Name], Email, Biography)
VALUES
('Marko Horvat', 'marko.horvat@example.com', 'Software developer specialized in C# and .NET.'),
('Ana Kovac', 'ana.kovac@example.com', 'Cybersecurity specialist with experience in network security.'),
('Ivan Novak', 'ivan.novak@example.com', 'Machine learning engineer and data science lecturer.');
go
INSERT INTO dbo.CourseInstructor (CourseId, InstructorId)
VALUES
(1, 1),
(2, 1),
(3, 2),
(4, 3);
go
SET IDENTITY_INSERT dbo.[Role] ON;
GO

INSERT INTO dbo.[Role] (Id, [Name])
VALUES
(1, 'Admin'),
(2, 'User');
GO

SET IDENTITY_INSERT dbo.[Role] OFF;
GO
INSERT INTO dbo.[Log] ([Timestamp], [Level], [Message], ErrorText)
VALUES
(SYSDATETIME(), 1, 'Database initialized with sample data.', NULL);