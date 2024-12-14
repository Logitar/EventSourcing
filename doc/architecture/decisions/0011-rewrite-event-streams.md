# 11. Rewrite the Framework with a Focus on Event Streams

Date: 2024-12-14

## Status

Accepted

## Context

We want to be able to use the [Decider pattern](https://thinkbeforecoding.com/post/2021/12/17/functional-event-sourcing-decider) (without `AggregateRoot`). We want to safeguard against concurrent updates to event streams.

## Decision

We did rewrite the framework while focusing on event streams, allowing both aggregates (Domain-Driven Design) and event streams (Event-Driven Architecture). We removed the support of an In-Memory event store, as well as MongoDB and relational event stores without EntityFrameworkCore. We added support of [EventStoreDB/Kurrent](https://www.eventstore.com/).

## Consequences

We can now use the Decider pattern and manipulate event streams without aggregates. We have dropped support to In-Memory, MongoDB and relational event stores without EntityFrameworkCore, which could impact clients who used those stores. We now safeguard against concurrent updates to events streams, which may require to rerun commands when event version conflict occur.
