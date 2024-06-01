-- Creation of SchoolSysDB
IF DB_ID('SchoolSysDB') IS NULL
BEGIN
    CREATE DATABASE SchoolSysDB;
END

-- Switching to SchoolSysDB
USE SchoolSysDB;

-- Checking of the existance of tables if that so drop them to avoid conflicts
IF OBJECT_ID('dbo.Class', 'U') IS NOT NULL DROP TABLE dbo.Class;
IF OBJECT_ID('dbo.Subjects', 'U') IS NOT NULL DROP TABLE dbo.Subjects;
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;
IF OBJECT_ID('dbo.Teachers', 'U') IS NOT NULL DROP TABLE dbo.Teachers;
IF OBJECT_ID('dbo.TeacherSubject', 'U') IS NOT NULL DROP TABLE dbo.TeacherSubject;
IF OBJECT_ID('dbo.TeacherAttendance', 'U') IS NOT NULL DROP TABLE dbo.TeacherAttendance;
IF OBJECT_ID('dbo.StudentAttendance', 'U') IS NOT NULL DROP TABLE dbo.StudentAttendance;
IF OBJECT_ID('dbo.Fees', 'U') IS NOT NULL DROP TABLE dbo.Fees;
IF OBJECT_ID('dbo.Exam', 'U') IS NOT NULL DROP TABLE dbo.Exam;
IF OBJECT_ID('dbo.Expense', 'U') IS NOT NULL DROP TABLE dbo.Expense;

-- Create Class table
CREATE TABLE Class (
    ClassId INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    ClassName VARCHAR(50) NOT NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL
);

-- Create Subjects table
CREATE TABLE Subjects (
    SubjectId INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    ClassId INT NULL,
    SubjectName VARCHAR(15) NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Class(ClassId)
);

-- Create Students table
CREATE TABLE Students (
    StudentId INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    Name VARCHAR(15) NULL,
    DOB DATE NULL,
    Gender VARCHAR(15) NULL,
    Mobile BIGINT NULL,
    RollNo VARCHAR(15) NULL,
    Address VARCHAR(MAX) NULL,
    ClassId INT NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Class(ClassId)
);

-- Create Teachers table
CREATE TABLE Teachers (
    TeacherId INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    Name VARCHAR(15) NULL,
    DOB DATE NULL,
    Gender VARCHAR(15) NULL,
    Mobile BIGINT NULL,
    Email VARCHAR(15) NULL,
    Address VARCHAR(MAX) NULL,
    Password VARCHAR(20) NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL
);

-- Create TeacherSubject table
CREATE TABLE TeacherSubject (
    Id INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    ClassId INT NULL,
    SubjectId INT NULL,
    TeacherId INT NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Class(ClassId),
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId),
    FOREIGN KEY (TeacherId) REFERENCES Teachers(TeacherId)
);

-- Create TeacherAttendance table
CREATE TABLE TeacherAttendance (
    Id INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    TeacherId INT NULL,
    Status BIT NULL,
    AttendanceDate DATE NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (TeacherId) REFERENCES Teachers(TeacherId)
);

-- Create StudentAttendance table
CREATE TABLE StudentAttendance (
    Id INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    ClassId INT NULL,
    SubjectId INT NULL,
    RollNo VARCHAR(20) NULL,
    Status BIT NULL,
    AttendanceDate DATE NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Class(ClassId),
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId)
);

-- Create Fees table
CREATE TABLE Fees (
    FeesId INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    ClassId INT NULL,
    FeesAmount INT NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Class(ClassId)
);

-- Create Exam table
CREATE TABLE Exam (
    ExamId INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    ClassId INT NULL,
    SubjectId INT NULL,
    RollNo VARCHAR(20) NULL,
    TotalMark INT NULL,
    OutOfMark INT NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Class(ClassId),
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId)
);

-- Create Expense table
CREATE TABLE Expense (
    ExpenseId INT PRIMARY KEY IDENTITY(1, 1) NOT NULL,
    ClassId INT NULL,
    SubjectId INT NULL,
    ChargeAmount INT NULL,
    created_at DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Class(ClassId),
    FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId)
);
