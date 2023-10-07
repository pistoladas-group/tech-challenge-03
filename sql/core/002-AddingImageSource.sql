USE TechNews
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230804220824_AddingImageSource')
BEGIN
    ALTER TABLE [News] ADD [ImageSource] VARCHAR(500) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230804220824_AddingImageSource')
BEGIN
    ALTER TABLE [Authors] ADD [ImageSource] VARCHAR(500) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230804220824_AddingImageSource')
BEGIN
    INSERT INTO [_AppliedMigrations] ([MigrationId], [ProductVersion])
    VALUES (N'20230804220824_AddingImageSource', N'7.0.3');
END;
GO

COMMIT;
GO