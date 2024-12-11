# Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL

Provides an implementation of a relational event store to be used with the Event Sourcing architecture pattern, Entity Framework Core and PostgreSQL.

## Migrations

This project is setup to use migrations. You must execute the following commands in the solution directory.

⚠️ Ensure the `EntityFrameworkCorePostgreSQL` database provider has been set in the database project user secrets.

### Create a new migration

Execute the following command to create a new migration. Do not forget to specify a migration name!

```sh
dotnet ef migrations add <YOUR_MIGRATION_NAME> --context EventContext --project lib/Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL --startup-project tools/Logitar.EventSourcing.Database
```

### Generate a script

Execute the following command to generate a new script. Do not forget to specify a source migration name!

```sh
dotnet ef migrations script <FROM_MIGRATION_NAME> --context EventContext --project lib/Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL --startup-project tools/Logitar.EventSourcing.Database
```
