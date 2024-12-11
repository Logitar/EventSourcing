IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF SCHEMA_ID(N'EventSourcing') IS NULL EXEC(N'CREATE SCHEMA [EventSourcing];');

CREATE TABLE [EventSourcing].[Streams] (
    [StreamId] bigint NOT NULL IDENTITY,
    [Id] nvarchar(255) NOT NULL,
    [Type] nvarchar(255) NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Streams] PRIMARY KEY ([StreamId])
);

CREATE TABLE [EventSourcing].[Events] (
    [EventId] bigint NOT NULL IDENTITY,
    [Id] nvarchar(255) NOT NULL,
    [StreamId] bigint NOT NULL,
    [Version] bigint NOT NULL,
    [ActorId] nvarchar(255) NULL,
    [OccurredOn] datetime2 NOT NULL,
    [IsDeleted] bit NULL,
    [TypeName] nvarchar(255) NOT NULL,
    [NamespacedType] nvarchar(255) NOT NULL,
    [Data] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY ([EventId]),
    CONSTRAINT [FK_Events_Streams_StreamId] FOREIGN KEY ([StreamId]) REFERENCES [EventSourcing].[Streams] ([StreamId]) ON DELETE CASCADE
);

CREATE INDEX [IX_Events_ActorId] ON [EventSourcing].[Events] ([ActorId]);

CREATE UNIQUE INDEX [IX_Events_Id] ON [EventSourcing].[Events] ([Id]);

CREATE INDEX [IX_Events_IsDeleted] ON [EventSourcing].[Events] ([IsDeleted]);

CREATE INDEX [IX_Events_OccurredOn] ON [EventSourcing].[Events] ([OccurredOn]);

CREATE UNIQUE INDEX [IX_Events_StreamId_Version] ON [EventSourcing].[Events] ([StreamId], [Version]);

CREATE INDEX [IX_Events_TypeName] ON [EventSourcing].[Events] ([TypeName]);

CREATE INDEX [IX_Events_Version] ON [EventSourcing].[Events] ([Version]);

CREATE INDEX [IX_Streams_CreatedBy] ON [EventSourcing].[Streams] ([CreatedBy]);

CREATE INDEX [IX_Streams_CreatedOn] ON [EventSourcing].[Streams] ([CreatedOn]);

CREATE UNIQUE INDEX [IX_Streams_Id] ON [EventSourcing].[Streams] ([Id]);

CREATE INDEX [IX_Streams_IsDeleted] ON [EventSourcing].[Streams] ([IsDeleted]);

CREATE INDEX [IX_Streams_Type] ON [EventSourcing].[Streams] ([Type]);

CREATE INDEX [IX_Streams_UpdatedBy] ON [EventSourcing].[Streams] ([UpdatedBy]);

CREATE INDEX [IX_Streams_UpdatedOn] ON [EventSourcing].[Streams] ([UpdatedOn]);

CREATE INDEX [IX_Streams_Version] ON [EventSourcing].[Streams] ([Version]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241211211432_CreateStreamAndEventTables', N'9.0.0');

COMMIT;
GO
