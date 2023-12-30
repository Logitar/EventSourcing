# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

Nothing yet.

## [5.0.0] - 2023-11-03

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

[unreleased]: https://github.com/Logitar/Logitar.NET/compare/v5.0.0...HEAD
[5.0.0]: https://github.com/Logitar/Logitar.NET/compare/v4.1.0...v5.0.0
[4.1.0]: https://github.com/Logitar/Logitar.NET/releases/tag/v4.1.0
