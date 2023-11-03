# Logitar.EventSourcing.MongoDB.IntegrationTests

This project provider integration tests for the Logitar.EventSourcing.MongoDB project. In order to
run the integration tests, you must have a running MongoDB server. The easiest way to do so is by
using Docker and running the following command, using the default values. The connection settings
are already configured in the `appsettings.json` file.

`docker run --name Logitar.EventSourcing_mongo -e MONGO_INITDB_ROOT_USERNAME=demo -e MONGO_INITDB_ROOT_PASSWORD=AwHE5MRKBeY9CsJu -p 7532:27017 -d mongo`
