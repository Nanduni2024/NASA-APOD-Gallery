IF DB_ID(N'NASA_APOD_DB') IS NULL
BEGIN
    CREATE DATABASE [NASA_APOD_DB];
END;
GO

USE [NASA_APOD_DB];
GO

IF OBJECT_ID(N'dbo.Apod', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Apod
    (
        [Date] date NOT NULL CONSTRAINT PK_Apod PRIMARY KEY,
        Title nvarchar(255) NOT NULL,
        Explanation nvarchar(max) NULL,
        Url nvarchar(2048) NOT NULL,
        MediaType nvarchar(50) NULL,
        ServiceVersion nvarchar(20) NULL,
        SavedAt datetime2(0) NOT NULL
            CONSTRAINT DF_Apod_SavedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF COL_LENGTH(N'dbo.Apod', N'SavedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Apod
    ADD SavedAt datetime2(0) NOT NULL
        CONSTRAINT DF_Apod_SavedAt DEFAULT SYSUTCDATETIME() WITH VALUES;
END;
GO