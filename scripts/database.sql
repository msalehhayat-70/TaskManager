-- ============================================================
--  Task Manager DB  –  Full Schema + Seed Script
--  Compatible: SQL Server 2019 / 2022
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskManagerDB')
    CREATE DATABASE TaskManagerDB;
GO

USE TaskManagerDB;
GO

-- ── Users ────────────────────────────────────────────────────────────────────
IF OBJECT_ID('Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        FullName     NVARCHAR(100)  NOT NULL,
        Email        NVARCHAR(200)  NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX)  NOT NULL,
        Role         NVARCHAR(20)   NOT NULL DEFAULT 'Member',
        CreatedAt    DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Table Users created.';
END
GO

-- ── Projects ─────────────────────────────────────────────────────────────────
IF OBJECT_ID('Projects', 'U') IS NULL
BEGIN
    CREATE TABLE Projects (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(200)  NOT NULL,
        Description NVARCHAR(1000) NULL,
        Status      INT            NOT NULL DEFAULT 0,  -- 0=Active,1=Completed,2=Archived
        Deadline    DATETIME2      NULL,
        OwnerId     INT            NOT NULL,
        CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Projects_Users FOREIGN KEY (OwnerId) REFERENCES Users(Id)
    );
    PRINT 'Table Projects created.';
END
GO

-- ── Tasks ─────────────────────────────────────────────────────────────────────
IF OBJECT_ID('Tasks', 'U') IS NULL
BEGIN
    CREATE TABLE Tasks (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Title        NVARCHAR(300) NOT NULL,
        Description  NVARCHAR(MAX) NULL,
        Priority     INT           NOT NULL DEFAULT 1,  -- 0=Low,1=Medium,2=High
        Status       INT           NOT NULL DEFAULT 0,  -- 0=Todo,1=InProgress,2=Done,3=Cancelled
        DueDate      DATETIME2     NULL,
        ProjectId    INT           NOT NULL,
        AssignedToId INT           NULL,
        CreatedAt    DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt    DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Tasks_Projects  FOREIGN KEY (ProjectId)    REFERENCES Projects(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Tasks_Users     FOREIGN KEY (AssignedToId) REFERENCES Users(Id)    ON DELETE SET NULL
    );
    PRINT 'Table Tasks created.';
END
GO

-- ── Seed Data ─────────────────────────────────────────────────────────────────
-- Admin user (password: Admin@123 — bcrypt hash)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@taskmanager.com')
BEGIN
    INSERT INTO Users (FullName, Email, PasswordHash, Role)
    VALUES ('Administrator', 'admin@taskmanager.com',
            '$2a$11$examplehashfordemopurposesonly1234567890AB', 'Admin');
    PRINT 'Seed: admin user inserted.';
END

IF NOT EXISTS (SELECT 1 FROM Projects WHERE Name = 'Sample Project')
BEGIN
    INSERT INTO Projects (Name, Description, OwnerId)
    VALUES ('Sample Project', 'Default demo project', 1);
    PRINT 'Seed: sample project inserted.';
END
GO

-- ── Useful Views ─────────────────────────────────────────────────────────────
CREATE OR ALTER VIEW vw_TaskSummary AS
    SELECT
        t.Id,
        t.Title,
        CASE t.Status WHEN 0 THEN 'Todo' WHEN 1 THEN 'In Progress'
                      WHEN 2 THEN 'Done'  ELSE 'Cancelled' END AS StatusLabel,
        CASE t.Priority WHEN 0 THEN 'Low' WHEN 1 THEN 'Medium' ELSE 'High' END AS PriorityLabel,
        p.Name  AS ProjectName,
        u.FullName AS AssignedTo,
        t.DueDate,
        t.CreatedAt
    FROM Tasks t
    JOIN Projects p ON p.Id = t.ProjectId
    LEFT JOIN Users u ON u.Id = t.AssignedToId;
GO

PRINT 'Database setup complete.';
