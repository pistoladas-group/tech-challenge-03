USE TechNews
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230816231535_ChangingPublishDateType')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[News]') AND [c].[name] = N'PublishDate');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [News] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [News] ALTER COLUMN [PublishDate] DATETIME NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230816231535_ChangingPublishDateType')
BEGIN
    INSERT INTO [_AppliedMigrations] ([MigrationId], [ProductVersion])
    VALUES (N'20230816231535_ChangingPublishDateType', N'7.0.3');
END;
GO

COMMIT;
GO