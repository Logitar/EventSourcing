﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'EventSourcing') THEN
        CREATE SCHEMA "EventSourcing";
    END IF;
END $EF$;

CREATE TABLE "EventSourcing"."Streams" (
    "StreamId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Id" character varying(255) NOT NULL,
    "Type" character varying(255),
    "Version" bigint NOT NULL,
    "CreatedBy" character varying(255),
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" character varying(255),
    "UpdatedOn" timestamp with time zone NOT NULL,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_Streams" PRIMARY KEY ("StreamId")
);

CREATE TABLE "EventSourcing"."Events" (
    "EventId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Id" character varying(255) NOT NULL,
    "StreamId" bigint NOT NULL,
    "Version" bigint NOT NULL,
    "ActorId" character varying(255),
    "OccurredOn" timestamp with time zone NOT NULL,
    "IsDeleted" boolean,
    "TypeName" character varying(255) NOT NULL,
    "NamespacedType" character varying(255) NOT NULL,
    "Data" text NOT NULL,
    CONSTRAINT "PK_Events" PRIMARY KEY ("EventId"),
    CONSTRAINT "FK_Events_Streams_StreamId" FOREIGN KEY ("StreamId") REFERENCES "EventSourcing"."Streams" ("StreamId") ON DELETE CASCADE
);

CREATE INDEX "IX_Events_ActorId" ON "EventSourcing"."Events" ("ActorId");

CREATE UNIQUE INDEX "IX_Events_Id" ON "EventSourcing"."Events" ("Id");

CREATE INDEX "IX_Events_IsDeleted" ON "EventSourcing"."Events" ("IsDeleted");

CREATE INDEX "IX_Events_OccurredOn" ON "EventSourcing"."Events" ("OccurredOn");

CREATE UNIQUE INDEX "IX_Events_StreamId_Version" ON "EventSourcing"."Events" ("StreamId", "Version");

CREATE INDEX "IX_Events_TypeName" ON "EventSourcing"."Events" ("TypeName");

CREATE INDEX "IX_Events_Version" ON "EventSourcing"."Events" ("Version");

CREATE INDEX "IX_Streams_CreatedBy" ON "EventSourcing"."Streams" ("CreatedBy");

CREATE INDEX "IX_Streams_CreatedOn" ON "EventSourcing"."Streams" ("CreatedOn");

CREATE UNIQUE INDEX "IX_Streams_Id" ON "EventSourcing"."Streams" ("Id");

CREATE INDEX "IX_Streams_IsDeleted" ON "EventSourcing"."Streams" ("IsDeleted");

CREATE INDEX "IX_Streams_Type" ON "EventSourcing"."Streams" ("Type");

CREATE INDEX "IX_Streams_UpdatedBy" ON "EventSourcing"."Streams" ("UpdatedBy");

CREATE INDEX "IX_Streams_UpdatedOn" ON "EventSourcing"."Streams" ("UpdatedOn");

CREATE INDEX "IX_Streams_Version" ON "EventSourcing"."Streams" ("Version");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241211211602_CreateStreamAndEventTables', '9.0.0');

COMMIT;
