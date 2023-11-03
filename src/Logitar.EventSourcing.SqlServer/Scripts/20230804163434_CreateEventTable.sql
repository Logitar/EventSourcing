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
GO

CREATE TABLE [Events] (
    [EventId] bigint NOT NULL IDENTITY,
    [Id] uniqueidentifier NOT NULL,
    [ActorId] nvarchar(255) NOT NULL,
    [IsDeleted] bit NULL,
    [OccurredOn] datetime2 NOT NULL,
    [Version] bigint NOT NULL,
    [AggregateType] nvarchar(255) NOT NULL,
    [AggregateId] nvarchar(255) NOT NULL,
    [EventType] nvarchar(255) NOT NULL,
    [EventData] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY ([EventId])
);
GO

CREATE INDEX [IX_Events_ActorId] ON [Events] ([ActorId]);
GO

CREATE INDEX [IX_Events_AggregateId] ON [Events] ([AggregateId]);
GO

CREATE INDEX [IX_Events_AggregateType_AggregateId] ON [Events] ([AggregateType], [AggregateId]);
GO

CREATE INDEX [IX_Events_EventType] ON [Events] ([EventType]);
GO

CREATE UNIQUE INDEX [IX_Events_Id] ON [Events] ([Id]);
GO

CREATE INDEX [IX_Events_IsDeleted] ON [Events] ([IsDeleted]);
GO

CREATE INDEX [IX_Events_OccurredOn] ON [Events] ([OccurredOn]);
GO

CREATE INDEX [IX_Events_Version] ON [Events] ([Version]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230804163434_CreateEventTable', N'7.0.9');
GO

COMMIT;
GO