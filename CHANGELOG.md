# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `EventBus.GetHandlersAsync` method.
- `EventBus` unit tests.

### Changed

- Improved `EventBus` implementation.

### Fixed

- GitHub Actions.
- LICENSE Year.
- NuGet upgrades.

## [10.0.0] - 2025-11-29

### Added

- `IEventHandler` interface.
- In-memory synchronous event handler.

### Changed

- Migrated to .NET10.

### Fixed

- NuGet upgrades.

## [7.0.2] - 2025-04-16

### Fixed

- NuGet upgrades.

## [7.0.1] - 2024-12-27

### Fixed

- Nuget publishing conflicts.
- Dependency Injection (services registered multiple times).

## [7.0.0] - 2024-12-14

There are so many changes in this version that I may have missed some.

### Added

- Documented upgrade to .NET 8, .NET 9 and rewriting of the framework.
- A lot of interfaces, such as `IAggregate` and `IEvent`, allowing to use your own implementation of those concepts.
- The `IEventStore` interface and its `EventStore` implementations.
- Integration of [EventStoreDB/Kurrent](https://www.eventstore.com/).
- Loading an aggregate from a snapshot.

### Changed

- Migrated to .NET9.
- Upgraded NuGet packages.
- Refactored domain exceptions.
- Rewrote the framework while focusing on event streams as well as aggregates. Previous versions database entities are not compatible with v7+.
- Aggregate `Apply` methods must be renamed to `Handle`, in accordance to C# event terminology.
- The `AggregateId` is now the `StreamId`.
- The `IAggregateRepository` and `AggregateRepository` have been renamed to `IRepository` and `Repository`.
- The `DomainEvent` is back to a _record_.
- Refactored exceptions.
- The `EventSerializer` can now be overloaded to register custom converters.
- Aggregate `LoadFromChanges` method is back to an instance method (not static anymore).
- The `IRepository` now allows loading deleted, undeleted and any aggregates (`includedDeleted` is now `isDeleted`, a nullable boolean).
- Rewrote unit and integration tests according to the framework changes.

### Fixed

- A bug when deserializing empty `ActorId`, `AggregateId/StreamId` and `EventId`.

### Removed

- Integration of in-memory, MongoDB, and relational event stores without EntityFrameworkCore.
- Actor identifier default value `SYSTEM`.

## [6.0.1] - 2024-11-28

### Added

- Added `publish.yaml`.

### Changed

- Renamed `docker-compose.yml` to `docker-compose.yaml`.

### Fixed

- Repository URL and Project URL in NuGet packages.
- Standardized `build.yml`.
- Upgraded NuGet packages.

## [6.0.0] - 2024-06-06

### Added

- Database migration tools.

### Changed

- `DomainEvent` is now a class, and uses an `EventId` struct.
- Using `ErrorMessageBuilder` in exceptions.
- Recreated scripts and migrations.
- Refactored the `LoadFromChanges<T>` method to remove Reflection.
- Core package now targets .NET Standard 2.1.
- `ToString` methods now include ID prefix.

### Fixed

- README files and migration commands.
- Docker Compose file.

## [5.2.0] - 2024-03-25

### Added

- Aggregate `Raise` methods.

### Changed

- Treat warnings as errors.
- Deprecated Events and added EventDb classes.
- NuGet update.

### Removed

- System usings.

## [5.1.1] - 2024-03-25

### Changed

- Added a name and renamed services in `docker-compose.yml`.

## [5.1.0] - 2024-01-10

### Added

- docker-compose.yml

### Changed

- Upgraded NuGet packages.
- Simplified integration test configurations.

### Fixed

- Updated LICENSE year.

## [5.0.0] - 2023-12-30

### Changed

- Upgraded to .NET8.

## [4.1.0] - 2023-11-03

### Added

- Implemented Event Sourcing with multiple data stores.
- Added AggregateRoot metadata.
- Created a struct for actor identifiers.
- Implemented an EventSourcing MongoDB store.
- Added Raise and Handle methods on AggregateRoot.

### Changed

- Reorganized the solution directory structure.
- Upgraded NuGet packages and fixed EventSourcing project dependencies.
- Replaced DeleteAction by a nullable boolean.
- Refactored AggregateRoot and DomainEvent.
- Injecting IEventSerializer as a dependency.
- Protected setters on aggregate metadata.
- Marked old TypeExtensions as obsolete.
- Refactored AggregateRoot.

[unreleased]: https://github.com/Logitar/EventSourcing/compare/v10.0.0...HEAD
[10.0.0]: https://github.com/Logitar/EventSourcing/compare/v7.0.2...v10.0.0
[7.0.2]: https://github.com/Logitar/EventSourcing/compare/v7.0.1...v7.0.2
[7.0.1]: https://github.com/Logitar/EventSourcing/compare/v7.0.0...v7.0.1
[7.0.0]: https://github.com/Logitar/EventSourcing/compare/v6.0.1...v7.0.0
[6.0.1]: https://github.com/Logitar/EventSourcing/compare/v6.0.0...v6.0.1
[6.0.0]: https://github.com/Logitar/EventSourcing/compare/v5.2.0...v6.0.0
[5.2.0]: https://github.com/Logitar/EventSourcing/compare/v5.1.1...v5.2.0
[5.1.1]: https://github.com/Logitar/EventSourcing/compare/v5.1.0...v5.1.1
[5.1.0]: https://github.com/Logitar/EventSourcing/compare/v5.0.0...v5.1.0
[5.0.0]: https://github.com/Logitar/EventSourcing/compare/v4.1.0...v5.0.0
[4.1.0]: https://github.com/Logitar/EventSourcing/releases/tag/v4.1.0
