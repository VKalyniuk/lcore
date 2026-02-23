# Lumini.Core.Cqrs

Lightweight CQRS infrastructure library for building modular and predictable .NET applications.

---

## Architectural Principles

Lumini.Core.Cqrs enforces a strict separation between:

- **Commands** — state mutation
- **Queries** — data retrieval
- **Notifications** — event propagation

The library is intentionally opinionated to guarantee atomicity and predictable execution flow.

---

## Command Handling

Commands represent state-changing operations.

> Each command must be handled by exactly **one** handler.

This ensures:

- Atomic execution
- Clear transactional boundaries
- Single responsibility point
- No competing business logic

One command → One handler → One state mutation.

Commands may optionally return a result.

---

## Query Handling

Queries represent read-only operations.

> Each query must be handled by exactly **one** handler.

This guarantees:

- Deterministic behavior
- No side effects
- Clear ownership of read logic

Queries must not modify system state.

One query → One handler → One response.

---

## Notifications (Events)

Notifications represent events that occurred in the system.

> A notification may be handled by **multiple** handlers.

This enables:

- Domain events
- Event-driven extensions
- Side effects such as logging, integrations, cache updates, email, projections, etc.

One notification → Zero or many handlers.

Notifications are designed for extensibility, not atomic state mutation.

---

## Atomicity by Design

The single-handler rule for Commands and Queries is intentional.

It provides:

- Predictable execution flow
- Clear reasoning about system behavior
- Strong transactional guarantees
- No implicit fan-out logic

State-changing and read operations are always isolated.

---

## Pipeline & Decorators

Handlers can be wrapped with decorators to introduce cross-cutting concerns:

- Validation
- Logging
- Transactions
- Retry policies
- Auditing
- Metrics

This keeps business logic clean and infrastructure concerns isolated.

---

## Summary

| Type | Handlers | Purpose |
|------|----------|----------|
| Command | Exactly 1 | Change state |
| Query | Exactly 1 | Read data |
| Notification | 0..N | React to events |

---

## Design Goals

- Modular architecture
- Clear intent modeling
- Predictable behavior
- Event-driven extensibility
- Infrastructure-agnostic business logic

---

Lumini.Core.Cqrs is built for systems where correctness, clarity, and explicit responsibility boundaries matter.