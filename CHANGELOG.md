# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- Renamed `docker-compose.yml` to `docker-compose.yaml`.

### Fixed

- Repository URL and Project URL in NuGet packages.
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

[unreleased]: https://github.com/Logitar/EventSourcing/compare/v6.0.0...HEAD
[6.0.0]: https://github.com/Logitar/EventSourcing/compare/v5.2.0...v6.0.0
[5.2.0]: https://github.com/Logitar/EventSourcing/compare/v5.1.1...v5.2.0
[5.1.1]: https://github.com/Logitar/EventSourcing/compare/v5.1.0...v5.1.1
[5.1.0]: https://github.com/Logitar/EventSourcing/compare/v5.0.0...v5.1.0
[5.0.0]: https://github.com/Logitar/EventSourcing/compare/v4.1.0...v5.0.0
[4.1.0]: https://github.com/Logitar/EventSourcing/releases/tag/v4.1.0
