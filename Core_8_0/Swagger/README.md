# Demo API - Architecture Documentation

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)]()

A demonstration RESTful API built with **.NET 8** showcasing enterprise-grade software architecture patterns, clean code principles, and industry best practices. This project serves as a reference implementation for building scalable, maintainable, and testable Web APIs.

---

## 📋 Table of Contents

- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Design Patterns & Principles](#design-patterns--principles)
- [Layer Responsibilities](#layer-responsibilities)
- [Testing Strategy](#testing-strategy)
- [Technologies & Libraries](#technologies--libraries)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)

---

## 🏗️ Architecture Overview

This project implements a **Clean Architecture** (also known as Onion Architecture), enforcing a strict separation of concerns where the **Domain** is the heart of the software.

### Dependency Inversion Principle (DIP)

A key characteristic of this architecture is the application of the **Dependency Inversion Principle**. 
- The **Domain Layer** defines the contracts (Interfaces) for data persistence and other external services.
- The **Infrastructure Layer** depends on the Domain and implements these interfaces.
- The **Application Layer** depends only on the Domain and abstractions, never on concrete infrastructure details.

This ensures that the core business logic remains agnostic to external technologies (like databases or APIs), making the system highly testable and adaptable.

### Architectural Diagram

```mermaid
graph TD
    subgraph Presentation [Presentation Layer]
        API[DemoApi.Api]
    end

    subgraph Application [Application Layer]
        App[DemoApi.Application]
    end

    subgraph Domain [Domain Layer]
        Dom[DemoApi.Domain]
    end

    subgraph Infrastructure [Infrastructure Layer]
        Data[DemoApi.Infra.Data]
        Cross[DemoApi.Infra.CrossCutting]
    end

    API --> App
    API --> Cross
    API --> Data
    App --> Dom
    Data --> Dom
    Cross --> Dom
```

*Note: The API references Infrastructure only for Dependency Injection (Composition Root).*

---

## 🎨 Design Patterns & Principles

### SOLID Principles
The project strictly adheres to SOLID principles, ensuring code is easy to maintain and extend.
- **SRP**: Classes have a single responsibility (e.g., `ProductAppService` orchestrates, `ProductRepository` persists).
- **OCP**: The system is open for extension (e.g., adding new repositories) but closed for modification.
- **DIP**: High-level modules (Application) do not depend on low-level modules (Infrastructure); both depend on abstractions (Domain Interfaces).

### Key Patterns Implemented

#### 1. **Repository Pattern**
Abstacts the data access logic. The domain defines `IProductRepository`, and the infrastructure implements it.
*Current implementation uses an In-Memory storage for demonstration purposes, but can be easily swapped for Entity Framework Core or Dapper without changing a single line of business logic.*

#### 2. **Notification Pattern (Domain Notifications)**
Instead of throwing exceptions for business validation errors (which is costly and breaks control flow), the project uses a `NotificatorHandler`.
- Errors are accumulated during the request.
- The controller checks for notifications before returning a response.
- Result: Consistent HTTP 400/422 responses with a standard error format.

#### 3. **Service Layer**
The `ProductAppService` acts as a facade for the domain. It orchestrates the flow:
1. Receives DTOs.
2. Maps to Domain Entities.
3. Validates business rules.
4. Calls Repositories.
5. Returns DTOs.

#### 4. **DTOs (Data Transfer Objects)**
`ProductViewModel` is used to decouple the internal Domain Entities from the external API contract. This allows the domain model to evolve independently of the API.

#### 5. **Custom Response Strategy**
The `MainApiController` implements a standardized response strategy to ensure consistency across all endpoints.
- **`CustomResponse(result)`**: Automatically wraps the result in the `ResponseViewModel` envelope.
- **Validation Handling**: Checks for domain notifications (`_notificator.HasErrors()`) before returning success. If errors exist, it returns `400 Bad Request` or `412 Precondition Failed` with the error list.
- **ModelState Integration**: The `CustomResponse(ModelState)` overload automatically extracts validation errors from ASP.NET Core's binding and adds them to the notification handler.

---

## 📁 Project Structure

```text
src/
├── DemoApi.Api/                    # Presentation Layer (Web API)
│   ├── Configuration/              # DI, Swagger, AutoMapper setup
│   ├── Controllers/                # API Endpoints
│   └── Program.cs                  # Composition Root
│
├── DemoApi.Application/            # Application Layer
│   ├── Services/                   # Business orchestration
│   ├── Models/                     # ViewModels/DTOs
│   └── Automapper/                 # Mapping profiles
│
├── DemoApi.Domain/                 # Domain Layer (Core)
│   ├── Entities/                   # Business Objects
│   ├── Interfaces/                 # Repository & Service Contracts
│   └── Handlers/                   # Notification Handler
│
├── DemoApi.Infra.Data/             # Infrastructure Layer
│   └── Repositories/               # Data access implementation
│
└── DemoApi.Infra.CrossCutting/     # Cross-Cutting Concerns
    └── Logging/                    # Logger implementation

tests/
├── DemoApi.Api.Test/               # Integration Tests
│   └── Factories/                  # WebApplicationFactory setup
│
└── DemoApi.Application.Test/       # Unit Tests
    └── Products/                   # Service logic tests
```

---

## 🧪 Testing Strategy

The project employs a comprehensive testing strategy ensuring reliability at all levels.

### 1. Unit Tests (`DemoApi.Application.Test`)
Focus on the **Application Layer** and **Business Rules**.
- **Tools**: xUnit, Moq, FluentAssertions, Bogus.
- **Strategy**: All external dependencies (Repositories, Notificator) are mocked. We test the logic in isolation (e.g., "Should return error if product name is empty").

### 2. Integration Tests (`DemoApi.Api.Test`)
Focus on the **API Endpoints** and the full request lifecycle.
- **Tools**: Microsoft.AspNetCore.Mvc.Testing (`WebApplicationFactory`).
- **Strategy**: Spins up an in-memory test server. Real HTTP requests are sent to the API to verify status codes, response bodies, and correct wiring of the dependency injection container.

---

## 🛠️ Technologies & Libraries

*   **Core**: [.NET 8](https://dotnet.microsoft.com/), C# 12
*   **Web API**: ASP.NET Core
*   **Mapping**: [AutoMapper](https://automapper.org/)
*   **Logging**: [NLog](https://nlog-project.org/)
*   **Documentation**: [Swagger / OpenAPI](https://swagger.io/)
*   **Testing**:
    *   [xUnit](https://xunit.net/)
    *   [Moq](https://github.com/moq/moq4)
    *   [FluentAssertions](https://fluentassertions.com/)
    *   [Bogus](https://github.com/bchavez/Bogus) (Fake data generation)

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/lucasbarbosa/Demo-Api.git
   cd Demo-Api
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the Application**
   ```bash
   dotnet run --project src/DemoApi.Api
   ```
   The API will start at `https://localhost:5001` (or similar, check console output).

4. **Run Tests**
   ```bash
   dotnet test
   ```

---

## 📖 API Documentation

The API is fully documented using Swagger/OpenAPI.

1. Run the application.
2. Navigate to: **`https://localhost:7167/swagger`** (port may vary).

### API Versioning Strategy

The API implements **URL Path Versioning** to ensure backward compatibility and smooth evolution of endpoints.

- **Format**: `/api/v{version}/{resource}`
- **Current Version**: `v1`
- **Default Behavior**: If no version is specified, the API assumes the default version (v1).

**Configuration Details:**
- `ReportApiVersions = true`: The API returns the supported versions in the response headers (`api-supported-versions`).
- `SubstituteApiVersionInUrl = true`: Swagger UI automatically handles the version parameter in the URL.

### Response Envelope Pattern

All API responses follow a consistent structure (Envelope Pattern), making it easier for clients to handle success and error states uniformly.

```json
{
    "success": true,
    "data": { ... },
    "errors": []
}
```

| Field | Type | Description |
|-------|------|-------------|
| `success` | `boolean` | Indicates if the operation was successful. |
| `data` | `object` | The payload of the response (null if error). |
| `errors` | `string[]` | List of error messages (business validations or exceptions). |

### HTTP Status Codes

| Status Code | Usage |
|-------------|-------|
| `200 OK` | Successful GET requests. |
| `201 Created` | Successful POST (create) requests. |
| `204 No Content` | Successful PUT/DELETE requests. |
| `400 Bad Request` | Business rule violations or invalid syntax. |
| `404 Not Found` | Resource not found. |
| `412 Precondition Failed` | Model validation errors (e.g., missing required fields). |
| `500 Internal Server Error` | Unexpected server errors. |

### Example Request (Create Product)

**POST** `/api/v1/products`

```json
{
  "name": "Premium Widget",
  "weight": 1.5
}
```

**Response (201 Created)**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Premium Widget",
    "weight": 1.5
  },
  "errors": []
}
```

---

## Layer Responsibilities

### Presentation Layer (`DemoApi.Api`)

**Responsibility:** Handle HTTP requests/responses and delegate to application services.

| Component | Purpose |
|-----------|---------|
| `MainApiController` | Base controller with standardized response handling |
| `ProductController` | RESTful endpoints for Product operations |
| `ExceptionMiddleware` | Global exception handling and logging |
| `ApiConfig` | API versioning and behavior configuration |
| `SwaggerConfig` | OpenAPI documentation setup |
| `DependencyInjectionConfig` | IoC container registration |

**Key Features:**
- **Global Exception Handling**: The `ExceptionMiddleware` intercepts unhandled exceptions, logs them using `NLog`, and returns a standardized `500 Internal Server Error` response in JSON format. This prevents sensitive stack traces from leaking to the client.
- **Configuration Extension Methods**: `Program.cs` is kept clean and readable by moving configuration logic into extension methods (e.g., `AddApiConfig`, `AddDependencyInjectionConfig`). This follows the "Convention over Configuration" approach and separates startup concerns.
- **API Versioning**: (`/api/v{version}/products`)
- **Standardized response envelope**: (`ResponseViewModel`)
- **Model validation**: with Data Annotations
- **Swagger/OpenAPI documentation**
