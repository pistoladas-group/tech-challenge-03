USE TechNewsAuth
GO

SET QUOTED_IDENTIFIER ON
GO

BEGIN TRANSACTION;
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

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE TABLE [Roles] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] VARCHAR(500) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [IsDeleted] bit NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] VARCHAR(500) NULL,
        [SecurityStamp] VARCHAR(500) NULL,
        [ConcurrencyStamp] VARCHAR(500) NULL,
        [PhoneNumber] VARCHAR(500) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE TABLE [RoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] uniqueidentifier NOT NULL,
        [ClaimType] VARCHAR(500) NULL,
        [ClaimValue] VARCHAR(500) NULL,
        CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE TABLE [UserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] VARCHAR(500) NULL,
        [ClaimValue] VARCHAR(500) NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE TABLE [UserLogins] (
        [LoginProvider] VARCHAR(500) NOT NULL,
        [ProviderKey] VARCHAR(500) NOT NULL,
        [ProviderDisplayName] VARCHAR(500) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]),
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE TABLE [UserTokens] (
        [UserId] uniqueidentifier NOT NULL,
        [LoginProvider] VARCHAR(500) NOT NULL,
        [Name] VARCHAR(500) NOT NULL,
        [Value] VARCHAR(500) NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [_AppliedMigrations] WHERE [MigrationId] = N'20230722221729_FirstMigration')
BEGIN
    INSERT INTO [_AppliedMigrations] ([MigrationId], [ProductVersion])
    VALUES (N'20230722221729_FirstMigration', N'7.0.3');
END;
GO

COMMIT;
GO