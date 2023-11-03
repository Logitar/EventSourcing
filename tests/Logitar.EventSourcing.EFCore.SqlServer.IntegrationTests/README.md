# Logitar.EventSourcing.EFCore.SqlServer.IntegrationTests

This project provider integration tests for the Logitar.EventSourcing.EntityFrameworkCore.SqlServer
project. In order to run the integration tests, you must have a running Microsoft SQL Server
database. The easiest way to do so is by using Docker and running the following command, using the
default values. The connection string is already configured in the `appsettings.json` file.

`docker run --name Logitar.EventSourcing_sqlserver -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=mWGEgJcrV5dzRyqb" -p 1435:1433 -d mcr.microsoft.com/mssql/server:2022-latest`
