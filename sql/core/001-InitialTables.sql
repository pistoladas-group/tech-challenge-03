USE TechNews
GO

IF OBJECT_ID(N'[_AppliedMigrations]') IS NULL
BEGIN
    CREATE TABLE [_AppliedMigrations] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK__AppliedMigrations] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230726165620_FirstMigration')
BEGIN
    CREATE TABLE [Authors] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Name] VARCHAR(100) NOT NULL,
        [Email] VARCHAR(100) NOT NULL,
        [CreatedAt] DATETIME NOT NULL,
        [IsDeleted] BIT NOT NULL,
        CONSTRAINT [PK_Authors] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230726165620_FirstMigration')
BEGIN
    CREATE TABLE [News] (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Title] VARCHAR(100) NOT NULL,
        [Description] VARCHAR(5000) NOT NULL,
        [PublishDate] VARCHAR(100) NOT NULL,
        [AuthorId] UNIQUEIDENTIFIER NOT NULL,
        [CreatedAt] DATETIME NOT NULL,
        [IsDeleted] BIT NOT NULL,
        CONSTRAINT [PK_News] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_News_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230726165620_FirstMigration')
BEGIN
    CREATE INDEX [IX_News_AuthorId] ON [News] ([AuthorId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230726165620_FirstMigration')
BEGIN
    INSERT INTO [_AppliedMigrations] ([MigrationId], [ProductVersion])
    VALUES (N'20230726165620_FirstMigration', N'7.0.3');
END;
GO

COMMIT;
GO