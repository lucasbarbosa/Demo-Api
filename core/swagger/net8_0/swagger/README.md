# Version 1: Clean Architecture Foundation

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-green)]()
[![Tests](https://img.shields.io/badge/Tests-73%20Passing-success)]()

> **Foundation implementation demonstrating Clean Architecture (Onion Architecture) with comprehensive testing strategy and SOLID principles**. This version establishes the architectural foundation for scalable, maintainable, and testable enterprise applications.

---

## 🎯 Overview

This is the **foundational version** of the Demo API project, implementing:

- ✅ **Clean Architecture** (Domain-Driven Design)
- ✅ **Repository Pattern** with abstraction
- ✅ **Dependency Inversion Principle** (DIP)
- ✅ **Comprehensive Test Coverage** (73 tests - 100% passing)
- ✅ **Structured Logging** (NLog)
- ✅ **API Documentation** (Swagger/OpenAPI)
- ✅ **Strongly Typed** (C# 12 features)

**Key Metrics**:
- **73 Tests** (22 Unit + 51 Integration)
- **100% Success Rate** (all tests passing)
- **~4,150 Lines of Code**
- **5 REST Endpoints** (CRUD operations)

---

## 🏗️ Clean Architecture Implementation

### Architecture Layers

This implementation follows **Clean Architecture** (also known as Onion Architecture), enforcing strict separation of concerns with the **Domain** at the core.

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                    │
│              (DemoApi.Api - Controllers)                │
│  - HTTP Request/Response handling                       │
│  - Middleware (Exception, Validation)                   │
│  - API Versioning                                       │
└────────────────────┬────────────────────────────────────┘
                     │ depends on
┌────────────────────▼────────────────────────────────────┐
│                  Application Layer                      │
│         (DemoApi.Application - Use Cases)               │
│  - Business workflows orchestration                     │
│  - DTOs/ViewModels                                      │
│  - AutoMapper profiles                                  │
└────────────────────┬────────────────────────────────────┘
                     │ depends on
┌────────────────────▼────────────────────────────────────┐
│                    Domain Layer                         │
│       (DemoApi.Domain - Business Rules) 💎              │
│  - Entities (Product)                                   │
│  - Interfaces (IProductRepository)                      │
│  - Domain handlers (NotificatorHandler)                 │
│  - ZERO external dependencies                           │
└─────────────────────────────────────────────────────────┘
                     ▲
                     │ implements
┌────────────────────┴────────────────────────────────────┐
│                Infrastructure Layer                     │
│  - DemoApi.Infra.Data (Repositories, EF Context)        │
│  - DemoApi.Infra.CrossCutting (Logging, DI)            │
└─────────────────────────────────────────────────────────┘
```

### Dependency Inversion Principle (DIP)

The hallmark of this architecture is **DIP application**:

| Layer | Responsibility | Dependencies | Example |
|-------|----------------|--------------|---------|
| **Domain** | Defines contracts (interfaces) | **None** (pure business logic) | `IProductRepository` interface |
| **Infrastructure** | Implements contracts | Domain (abstractions only) | `ProductRepository : IProductRepository` |
| **Application** | Orchestrates use cases | Domain (interfaces only) | `ProductAppService` uses `IProductRepository` |
| **Presentation** | Handles HTTP | Application + CrossCutting | `ProductController` uses `IProductAppService` |

**Benefits**:
- ✅ **Testability**: Business logic testable without infrastructure
- ✅ **Flexibility**: Swap database/framework without touching business rules
- ✅ **Low Coupling**: Each layer knows only abstractions
- ✅ **High Cohesion**: Well-defined responsibilities

---

## 📂 Project Structure

```
swagger/
├── src/
│   ├── DemoApi.Api/                    # 🌐 Presentation Layer
│   │   ├── Configuration/
│   │   │   ├── ApiConfig.cs           # API versioning, MVC setup
│   │   │   ├── DependencyInjectionConfig.cs  # IoC container
│   │   │   ├── NLogConfig.cs          # Logging configuration
│   │   │   └── SwaggerConfig.cs       # OpenAPI documentation
│   │   ├── Controllers/
│   │   │   └── MainApiController.cs   # Base controller (response standardization)
│   │   ├── Extensions/
│   │   │   ├── ExceptionMiddleware.cs # Global exception handling
│   │   │   └── ModelValidationFilter.cs # Model state validation
│   │   ├── V1/Controllers/
│   │   │   └── ProductController.cs   # Products CRUD endpoints
│   │   └── Program.cs                 # Entry point
│   │
│   ├── DemoApi.Application/            # 📋 Application Layer
│   │   ├── Automapper/
│   │   │   └── AutomapperConfig.cs    # Entity ↔ ViewModel mappings
│   │   ├── Interfaces/
│   │   │   └── IProductAppService.cs  # Application service contract
│   │   ├── Models/
│   │   │   ├── BaseViewModel.cs
│   │   │   ├── ResponseViewModel.cs   # Standardized API responses
│   │   │   └── Products/
│   │   │       ├── ProductViewModel.cs     # Product DTO
│   │   │       ├── ProductResponse.cs      # Single product response
│   │   │       └── ProductListResponse.cs  # Collection response
│   │   └── Services/
│   │       └── ProductAppService.cs   # Use cases implementation
│   │
│   ├── DemoApi.Domain/                 # 💎 Domain Layer (Core)
│   │   ├── Entities/
│   │   │   └── Product.cs             # Domain entity
│   │   ├── Handlers/
│   │   │   ├── Notification.cs        # Error notification
│   │   │   └── NotificatorHandler.cs  # Notification pattern
│   │   └── Interfaces/
│   │       ├── IProductRepository.cs  # Repository contract
│   │       └── INotificatorHandler.cs # Notification contract
│   │
│   ├── DemoApi.Infra/                  # 🔧 Infrastructure (Data)
│   │   ├── Context/
│   │   │   └── InMemoryDbContext.cs   # EF Core in-memory database
│   │   └── Repositories/
│   │       └── ProductRepository.cs   # Repository implementation
│   │
│   └── DemoApi.Infra.CrossCutting/     # 🔧 Infrastructure (Cross-Cutting)
│       ├── Interfaces/
│       │   └── ILogger.cs             # Logging abstraction
│       ├── Logging/
│       │   └── NLogLogger.cs          # NLog implementation
│       └── nlog.config                # NLog configuration
│
└── tests/
    ├── DemoApi.Api.Test/               # 🧪 Integration Tests (51 tests)
    │   ├── Configuration/
    │   │   ├── CustomWebApplicationFactory.cs  # Test server factory
    │   │   ├── PriorityOrderer.cs              # Test execution order
    │   │   └── TestPriorityAttribute.cs        # Priority annotation
    │   ├── Factories/
    │   │   └── (test data factories)
    │   ├── Filters/
    │   │   └── ModelValidationFilterTests.cs   # Validation filter tests
    │   ├── Helpers/
    │   │   └── HttpClientHelper.cs             # HTTP test helper (tuples)
    │   ├── Middleware/
    │   │   └── ExceptionMiddlewareTests.cs     # Exception handling tests
    │   ├── Products/
    │   │   ├── ProductTests.cs                 # Base test class (14 helpers)
    │   │   ├── CreateProductTests.cs           # POST endpoint tests
    │   │   ├── GetProductTests.cs              # GET endpoints tests
    │   │   ├── UpdateProductTests.cs           # PUT endpoint tests
    │   │   └── DeleteProductTests.cs           # DELETE endpoint tests
    │   └── xunit.runner.json                   # Anti-parallelism config
    │
    └── DemoApi.Application.Test/       # 🧪 Unit Tests (22 tests)
        └── Services/
            └── ProductAppServiceTests.cs       # Service layer tests
```

---

## 🧪 Testing Strategy

### Test Pyramid

This project implements a **balanced test pyramid**:

```
           ┌─────────────────┐
           │  Integration    │  51 tests (70%)
           │  Tests          │  - Full HTTP pipeline
           │                 │  - End-to-end scenarios
           ├─────────────────┤
           │  Unit Tests     │  22 tests (30%)
           │                 │  - Service layer
           │                 │  - Business logic
           └─────────────────┘
```

### Test Coverage Breakdown

| Test Suite | Tests | Coverage | Purpose |
|------------|:-----:|----------|---------|
| **CreateProductTests** | 12 | POST endpoint | Creation validation, duplicates, edge cases |
| **GetProductTests** | 11 | GET endpoints | Retrieval, filtering, not found scenarios |
| **UpdateProductTests** | 11 | PUT endpoint | Update validation, concurrency, partial updates |
| **DeleteProductTests** | 7 | DELETE endpoint | Deletion, idempotency, cascading effects |
| **ModelValidationFilterTests** | 5 | Validation filter | Model state validation |
| **ExceptionMiddlewareTests** | 5 | Exception middleware | Global error handling |
| **ProductAppServiceTests** | 22 | Service layer | Business logic, repository interaction |
| **TOTAL** | **73** | **100% Passing** | Full CRUD + validation + error handling |

### Testing Best Practices Implemented

#### 1. **AAA Pattern** (Arrange-Act-Assert)

Every test follows the AAA pattern for clarity:

```csharp
[Fact]
public async Task Create_ShouldReturnCreated_WhenProductIsValid()
{
    // Arrange
    string url = "/api/v1/products";
    ProductViewModel productFake = NewProduct();

    // Act
    (HttpResponseMessage response, ResponseViewModel? viewModel) = 
        await HttpClientHelper.PostAndReturnResponseAsync(_client, url, productFake);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    viewModel!.Success.Should().BeTrue();
    viewModel!.Data.Should().NotBeNull();
}
```

#### 2. **Strongly Typed Variables** (C# 12)

No `var` keyword - all variables explicitly typed:

```csharp
// ❌ Avoided
var product = await GetLastCreatedProduct();

// ✅ Applied
ProductViewModel product = await GetLastCreatedProduct();
IList<ProductViewModel> products = await _productApplication.GetAll();
HttpResponseMessage response = await _client.GetAsync(url);
```

#### 3. **HttpClientHelper Modernization**

Custom helper using **tuple returns** for clean assertions:

```csharp
public static class HttpClientHelper
{
    public static async Task<(HttpResponseMessage, ResponseViewModel?)> 
        GetAndReturnResponseAsync(HttpClient client, string url)
    {
        HttpResponseMessage response = await client.GetAsync(url);
        ResponseViewModel? viewModel = await response.Content
            .ReadFromJsonAsync<ResponseViewModel>();
        return (response, viewModel);
    }
}

// Usage
(HttpResponseMessage response, ResponseViewModel? viewModel) = 
    await HttpClientHelper.GetAndReturnResponseAsync(_client, url);
```

#### 4. **Test Base Class with 14 Reusable Helpers**

`ProductTests` base class provides factory methods:

```csharp
protected static ProductViewModel NewProduct()                     // Valid product (Bogus)
protected static ProductViewModel ProductWithUniqueName()          // Unique name
protected static ProductViewModel NonExistentProduct()             // ID 999999
protected static ProductViewModel ProductWithEmptyName()           // Name = ""
protected static ProductViewModel ProductWithNullName()            // Name = null
protected static ProductViewModel ProductWithZeroWeight()          // Weight = 0
protected static ProductViewModel ProductWithNegativeWeight()      // Weight < 0
protected static ProductViewModel ProductWithLongName()            // Name = 100 chars
protected static ProductViewModel ProductWithLargeWeight()         // Weight = 1,000,000
protected static ProductViewModel ProductWithIdZero()              // ID = 0
protected static ProductViewModel ProductToUpdate(...)            // Update template
protected static ProductViewModel ProductToUpdateWeightOnly(...)  // Partial update
protected static ProductViewModel ProductToUpdateNameOnly(...)    // Partial update
protected async Task<ProductViewModel> GetLastCreatedProduct()    // Create + return
```

#### 5. **Anti-Parallelism Configuration**

Tests run **sequentially** for stability (shared in-memory database):

```json
// xunit.runner.json
{
  "maxParallelThreads": 1,
  "parallelizeAssembly": false,
  "parallelizeTestCollections": false
}
```

#### 6. **Test Prioritization**

Tests execute in controlled order using `TestPriorityAttribute`:

```csharp
[TestCaseOrderer("DemoApi.Api.Test.Configuration.PriorityOrderer", "DemoApi.Api.Test")]
public class CreateProductTests : ProductTests
{
    [Fact, TestPriority(100)]
    public async Task Create_ShouldReturnCreated_WhenProductIsValid() { }

    [Fact, TestPriority(101)]
    public async Task Create_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty() { }
}
```

### Test Technologies

| Library | Version | Purpose |
|---------|---------|---------|
| **xUnit** | 2.5.3 | Test framework (industry standard for .NET) |
| **FluentAssertions** | 8.8.0 | Readable assertions (`Should().Be()`) |
| **Moq** | 4.20.72 | Mocking framework (repository mocks) |
| **Bogus** | 34.0.2 | Fake data generation (realistic test data) |
| **Microsoft.AspNetCore.Mvc.Testing** | 8.0.22 | Integration test host (`WebApplicationFactory`) |
| **Microsoft.AspNetCore.TestHost** | 8.0.22 | In-memory test server |
| **coverlet.collector** | 6.0.0 | Code coverage metrics |

---

## 🛠️ Technology Stack

### Core Framework

| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 8.0 (LTS until Nov 2026) | Runtime framework |
| **C#** | 12.0 | Language (Primary Constructors, Collection Expressions) |
| **ASP.NET Core** | 8.0 | Web framework |

### Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| **AutoMapper.Extensions.Microsoft.DependencyInjection** | 12.0.1 | Object-to-object mapping (Entity ↔ DTO) |
| **Swashbuckle.AspNetCore** | 6.6.2 | Swagger/OpenAPI documentation |
| **Microsoft.AspNetCore.Mvc.Versioning** | 5.0.0 | API versioning (URL-based: `/api/v1`) |
| **Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer** | 5.0.0 | API explorer for Swagger integration |
| **NLog.Web.AspNetCore** | 6.1.0 | Structured logging (file + console) |
| **Newtonsoft.Json** | 13.0.4 | JSON serialization/deserialization |

---

## 📡 API Endpoints

### Products CRUD

All endpoints return standardized responses via `ResponseViewModel`:

```json
{
  "success": true,
  "data": { /* entity or collection */ },
  "errors": []
}
```

| Method | Endpoint | Request Body | Success Response | Error Codes |
|--------|----------|--------------|------------------|-------------|
| `GET` | `/api/v1/products` | - | `200 OK` - `ProductListResponse` | `400` |
| `GET` | `/api/v1/products/{id}` | - | `200 OK` - `ProductResponse` | `400`, `404` |
| `POST` | `/api/v1/products` | `ProductViewModel` | `201 Created` - `ProductResponse` | `400`, `412` |
| `PUT` | `/api/v1/products` | `ProductViewModel` | `204 No Content` | `400`, `404`, `412` |
| `DELETE` | `/api/v1/products/{id}` | - | `204 No Content` | `400`, `404` |

#### ProductViewModel Schema

```json
{
  "id": 0,
  "name": "string (required, 1-255 chars)",
  "weight": 0.01
}
```

**Validations** (Data Annotations):
- `Name`: `[Required]` - Cannot be null or empty
- `Weight`: `[Range(0.01, double.MaxValue)]` - Must be greater than 0

#### HTTP Status Codes

| Code | Meaning | When Occurs |
|------|---------|-------------|
| `200 OK` | Success (read) | Product(s) found |
| `201 Created` | Resource created | POST successful |
| `204 No Content` | Success (no body) | PUT/DELETE successful |
| `400 Bad Request` | Invalid input | Malformed request, type mismatch |
| `404 Not Found` | Resource missing | Product doesn't exist |
| `412 Precondition Failed` | Validation failed | Data Annotations violation |
| `500 Internal Server Error` | Server error | Unhandled exception |

### Example: Complete CRUD Flow

```bash
# 1. Create Product
curl -X POST http://localhost:5001/api/v1/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop Dell XPS 15",
    "weight": 2.5
  }'

# Response: 201 Created
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Laptop Dell XPS 15",
    "weight": 2.5
  }
}

# 2. Get All Products
curl http://localhost:5001/api/v1/products

# Response: 200 OK
{
  "success": true,
  "data": [
    { "id": 1, "name": "Laptop Dell XPS 15", "weight": 2.5 }
  ]
}

# 3. Get Product by ID
curl http://localhost:5001/api/v1/products/1

# Response: 200 OK
{
  "success": true,
  "data": { "id": 1, "name": "Laptop Dell XPS 15", "weight": 2.5 }
}

# 4. Update Product
curl -X PUT http://localhost:5001/api/v1/products \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "name": "Laptop Dell XPS 15 (Updated)",
    "weight": 2.7
  }'

# Response: 204 No Content

# 5. Delete Product
curl -X DELETE http://localhost:5001/api/v1/products/1

# Response: 204 No Content
```

---

## 🚀 Quick Start

### Prerequisites

- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **IDE**: Visual Studio 2022, VS Code, or Rider

### Running the API

```bash
# Clone repository
git clone https://github.com/lucasbarbosa/demo-api.git
cd demo-api/net8_0/swagger

# Restore dependencies
dotnet restore

# Run API
dotnet run --project src/DemoApi.Api

# API available at:
# - HTTPS: https://localhost:5001
# - HTTP: http://localhost:5000
# - Swagger UI: https://localhost:5001/swagger
```

### Running Tests

```bash
# All tests
dotnet test

# With detailed output
dotnet test --logger "console;verbosity=detailed"

# With code coverage
dotnet test --collect:"XPlat Code Coverage"

# Specific test project
dotnet test tests/DemoApi.Api.Test
dotnet test tests/DemoApi.Application.Test
```

**Expected Output**:
```
Passed!  - Failed:     0, Passed:    73, Skipped:     0, Total:    73
```

---

## 📊 Code Quality Metrics

### Test Metrics

| Metric | Value | Target |
|--------|:-----:|:------:|
| **Total Tests** | 73 | - |
| **Success Rate** | 100% | 100% |
| **Unit Tests** | 22 (30%) | 20-40% |
| **Integration Tests** | 51 (70%) | 60-80% |
| **Execution Time** | ~1.4s | <5s |
| **Code Coverage** | ~85% | >80% |

### Code Metrics

| Metric | Value |
|--------|-------|
| **Total Lines of Code** | ~4,150 |
| **API Endpoints** | 5 (CRUD) |
| **Projects** | 7 (4 src + 2 tests + 1 domain) |
| **Build Time** | ~30s |
| **Warnings** | 0 |
| **Code Smells** | 0 |

### Quality Indicators

- ✅ **100% Build Success** - Zero compilation errors
- ✅ **Zero Warnings** - Clean compilation
- ✅ **SOLID Principles** - Applied throughout
- ✅ **DRY Principle** - No code duplication
- ✅ **Strongly Typed** - No `var` in tests
- ✅ **Comprehensive Tests** - All scenarios covered

---

## 🎓 Key Learning Objectives

This implementation demonstrates:

### Architecture Patterns
- ✅ **Clean Architecture** (Onion Architecture)
- ✅ **Repository Pattern** with abstraction
- ✅ **Dependency Inversion Principle**
- ✅ **Notification Pattern** (avoid exceptions for business rules)

### .NET 8 Features
- ✅ **Primary Constructors** (C# 12)
- ✅ **Collection Expressions** (C# 12)
- ✅ **Nullable Reference Types**
- ✅ **Top-Level Statements** (Program.cs)

### Testing Strategies
- ✅ **Test Pyramid** (Unit + Integration)
- ✅ **AAA Pattern** (Arrange-Act-Assert)
- ✅ **Test Fixtures** (base class with helpers)
- ✅ **In-Memory Testing** (WebApplicationFactory)

### Best Practices
- ✅ **Dependency Injection** (built-in .NET DI)
- ✅ **AutoMapper** (entity-to-DTO mapping)
- ✅ **Structured Logging** (NLog)
- ✅ **API Documentation** (Swagger/OpenAPI)
- ✅ **Standardized Responses** (ResponseViewModel)

---

## 🔗 Navigation

### Project Hierarchy

- 📘 [**← Main README**](../../README.md) - Project overview & version comparison
- 📘 [**Version 2 (JWT) →**](../swagger-jwt/README.md) - Add JWT authentication + FluentValidation
- 📘 [**Version 3 (Docker) →**](../swagger-jwt-docker/README.md) - Add Docker containerization

### Related Documentation

- 📄 [AutoMapper Configuration](src/DemoApi.Application/Automapper/AutomapperConfig.cs)
- 📄 [Dependency Injection Setup](src/DemoApi.Api/Configuration/DependencyInjectionConfig.cs)
- 📄 [Exception Middleware](src/DemoApi.Api/Extensions/ExceptionMiddleware.cs)
- 📄 [Test Base Class](tests/DemoApi.Api.Test/Products/ProductTests.cs)

---

## 📄 License

This project is licensed under the MIT License - see [LICENSE](../../LICENSE) for details.

---

<div align="center">

**Built with ❤️ using .NET 8 and Clean Architecture principles**

*Version 1 of 3 - Foundation Layer*

[← Back to Main](../../README.md) | [Next: JWT Security →](../swagger-jwt/README.md)

</div>

---
