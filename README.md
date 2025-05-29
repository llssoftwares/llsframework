# LLSFramework

LLSFramework is a modular .NET 9 solution designed to accelerate the development of modern web applications using ASP.NET Core, Blazor, SignalR, and robust authentication mechanisms. The framework provides reusable components, service extensions, and integration patterns for scalable, real-time, and secure applications.

---

## LLSFramework.Core

A foundational library providing domain-driven design (DDD) patterns, dynamic filtering and parsing, and utility extensions.

### Key Features

- **Domain-Driven Design (DDD) Support:**
  - Base classes and interfaces for aggregates, entities, and value objects.
  - Promotes clean separation of domain logic and infrastructure.

- **Filtering and Pagination:**
  - Utilities for building and applying filters to queries implementing an attribute-based approach.
  - Pagination helpers for efficient data retrieval and UI integration.

- **Generic Parsing:**
  - Strongly-typed parsing for primitives, enums, nullable types, and collections.
  - Supports parsing from strings to complex types, including lists and enums.
  - Extensively unit-tested for reliability.

- **Extension Methods:**
  - Enhancements for reflection, collections, expressions and other .NET types to streamline common operations.

## LLSFramework.Application

This library provides core application services for .NET 9 solutions, focusing on authentication, authorization, and OpenAPI documentation integration. 

### Key Features

- **JWT Authentication and Token Management:**
  - Provides robust JWT (JSON Web Token) generation and validation using application-specific settings.  
  - Uses `JwtSettings` (bound from configuration) for issuer, audience, secret key, and expiration.
  - Utilizes HMAC SHA-256 for signing and enforces strict validation parameters (e.g., zero clock skew).
  - Registered as a scoped service and used to configure JWT Bearer authentication for both API and SignalR endpoints.
 
- **OpenAPI/Swagger Runtime Transformation:**
  - Dynamically transforms the OpenAPI (Swagger) document at runtime to reflect current server settings and security requirements.
  - Scans all API endpoints and automatically applies security requirements to operations that require authorization (based on endpoint metadata).
  - Uses route and HTTP method matching to accurately associate OpenAPI operations with ASP.NET Core endpoints.
  - Registered as a document transformer in the OpenAPI/Swagger pipeline, ensuring documentation always matches the app’s runtime configuration and security.
 
- **Service Registration and Middleware Integration:**
  - Registers JWT token management, configures authentication/authorization, and sets up OpenAPI documentation.
  - Configures JWT Bearer authentication, including support for SignalR by extracting tokens from query strings for hub endpoints.
  - Registers OpenAPI/Swagger with the custom document transformer for runtime customization.
  - Maps endpoints for OpenAPI and Scalar API reference UI, providing interactive API documentation.
 
- **Design and Extensibility:**
  - All critical settings (JWT, OpenAPI servers, SignalR hub base URLs) are loaded from configuration, supporting flexible deployment scenarios.
  - Utilizes dependency injection, options pattern, and endpoint metadata for clean, maintainable code.
  - JWT authentication is seamlessly integrated with SignalR, enabling secure real-time communication.

## LLSFramework.TabBlazor

A Blazor component library that extends Tabler and TabBlazor features, providing advanced UI features, real-time communication, and modal/dialog management.

### Key Features

- **SignalR Integration (`SignalRComponentBase<T>`):**
  - Abstract base for Blazor components that interact with SignalR hubs.
  - Manages connection lifecycle, authentication (JWT), and event wiring.
  - Supports group-based messaging and custom event handlers.
  - Handles automatic reconnection and resource cleanup.

- **Dynamic Navbar (`LLSNavbar`):**
  - Responsive navigation bar supporting horizontal and vertical layouts.
  - Manages menu items, dropdowns, and navigation events.
  - Provides methods for toggling, expanding, and closing menus programmatically.

- **Modal Dialog Builder (`ModalBuilder<TComponent>`):**
  - Fluent API for configuring and displaying modals with custom Blazor components.
  - Supports options like size, position, header, close behavior, scrollability, backdrop, drag, and custom CSS.
  - Allows parameter passing to modal content and returns results asynchronously.

- **Lookup List Component (`LookupListComponentBase`):**
  - Abstract base for multi-select lookup lists with modal-based searching.
  - Supports formatted display of selected items and event-driven updates.

- **Service Registration Extensions:**
  - Extension methods for registering all core services, including authentication, SignalR, UI utilities, and custom user ID providers.
