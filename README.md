# Character Explorer

Character Explorer is built using Onion Architecture and Domain-Driven Design (DDD). The project separates the domain, application services, infrastructure, and presentation concerns so that each part can evolve independently.

## Architecture

The dependency inversion and separation of concerns in this project make it straightforward to replace both the persistence and presentation layers. Dependencies are supplied through dependency injection, allowing the core business logic to remain independent of implementation details such as databases, web clients, or UI frameworks.

This repository includes two persistence implementations:

- An in-memory persistence implementation for local development and testing.
- A persistence implementation that retrieves character data from the [SWAPI](https://swapi.dev/) API.

Because these implementations are decoupled from the domain and application logic, one can be substituted for the other without changing the business rules.

## Testing

The separation of concerns also supports focused unit testing. Tests can exercise the business logic without external dependencies by using the in-memory persistence implementation or suitable test doubles. This keeps the tests fast, deterministic, and focused solely on the behavior implemented by the application.

In the future, broader end-to-end tests can verify the API or UI while instantiating the application with any repository implementation, including repositories designed to throw errors, so that integration behavior and failure paths are covered as well.
