# FluentValidation Implementation Guide

## 📋 Overview

This document explains the **FluentValidation** implementation in DemoApi, covering migration rationale, patterns, testing strategies, and best practices.

---

## 🎯 Why FluentValidation?

### Migration Rationale

**Before (Data Annotations)**:
```csharp
public class ProductViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
    public double Weight { get; set; }
}
```

**After (FluentValidation)**:
```csharp
// Clean model
public class ProductViewModel
{
    public string Name { get; set; }
    public double Weight { get; set; }
}

// Dedicated validator
public class ProductValidator : AbstractValidator<ProductViewModel>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(p => p.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0");
    }
}
```

### Comparison Matrix

| Feature | Data Annotations | FluentValidation | Winner |
|---------|------------------|------------------|--------|
| **Separation of Concerns** | ❌ Coupled to models | ✅ Separate classes | FluentValidation |
| **Testability** | ⚠️ Requires model creation | ✅ Direct validator testing | FluentValidation |
| **Complex Rules** | ⚠️ Limited | ✅ Unlimited flexibility | FluentValidation |
| **Async Validation** | ❌ Not supported | ✅ `MustAsync()` | FluentValidation |
| **Conditional Logic** | ⚠️ Very difficult | ✅ `When()`, `Unless()` | FluentValidation |
| **Reusability** | ❌ Tight coupling | ✅ Inheritance/composition | FluentValidation |
| **Learning Curve** | ✅ Low | ⚠️ Moderate | Data Annotations |
| **Performance** | ✅ Slightly faster | ✅ Negligible difference | Tie |

**Decision**: FluentValidation chosen for **scalability**, **testability**, and **maintainability**.

---

## 🏗️ Implementation Pattern

### Basic Validator

**File**: `src/DemoApi.Application/Validators/Products/ProductValidator.cs`

```csharp
using DemoApi.Application.Models.Products;
using FluentValidation;

namespace DemoApi.Application.Validators.Products
{
    public class ProductValidator : AbstractValidator<ProductViewModel>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name is required");

            RuleFor(p => p.Weight)
                .GreaterThan(0)
                .WithMessage("Weight must be greater than 0");
        }
    }
}
```

### Key Components

1. **Inheritance**: `AbstractValidator<ProductViewModel>`
2. **Fluent API**: `RuleFor().NotEmpty().WithMessage()`
3. **Explicit Messages**: Custom error messages matching business requirements
4. **Single Responsibility**: Validator only handles validation logic

---

## 🔧 Dependency Injection Setup

### Configuration

**File**: `src/DemoApi.Api/Configuration/DependencyInjectionConfig.cs`

```csharp
public static IServiceCollection AddDependencyInjectionConfig(this IServiceCollection services)
{
    #region FluentValidation

    // Auto-validation on model binding
    services.AddFluentValidationAutoValidation();
    
    // Client-side validation adapters (optional)
    services.AddFluentValidationClientsideAdapters();
    
    // Auto-register all validators in assembly
    services.AddValidatorsFromAssemblyContaining<ProductValidator>();

    #endregion

    // ... other services
}
```

### Registration Strategy

- **Auto-registration**: Assembly scanning finds all `AbstractValidator<T>` classes
- **Scoped lifetime**: Validators support constructor injection
- **Convention over configuration**: Zero manual registration required

---

## 🔄 Validation Pipeline

### Request Flow

```
HTTP Request (JSON)
    ↓
Model Binding (JSON → ProductViewModel)
    ↓
FluentValidation.AspNetCore
    ↓
ProductValidator.Validate()
    ↓
┌─────────────┐
│ Valid?      │
└─────────────┘
     │
     ├─Yes─→ Controller Action
     │
     └─No──→ ModelValidationFilter
                ↓
           412 Precondition Failed
           {
             "success": false,
             "errors": ["Name is required", ...]
           }
```

### Error Response Structure

**Request**:
```http
POST /api/v1/products
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "",
  "weight": -5
}
```

**Response** (412):
```json
{
  "success": false,
  "data": null,
  "errors": [
    "Name is required",
    "Weight must be greater than 0"
  ]
}
```

---

## 🎨 Advanced Validation Patterns

### 1. Conditional Validation

```csharp
public class ProductValidator : AbstractValidator<ProductViewModel>
{
    public ProductValidator()
    {
        RuleFor(p => p.Weight)
            .GreaterThan(0)
            .When(p => p.Category != "Digital") // Only for physical products
            .WithMessage("Weight is required for physical products");
    }
}
```

### 2. Cross-Property Validation

```csharp
RuleFor(p => p.Discount)
    .LessThanOrEqualTo(p => p.Price * 0.5m)
    .WithMessage("Discount cannot exceed 50% of price");

RuleFor(p => p.EndDate)
    .GreaterThan(p => p.StartDate)
    .WithMessage("End date must be after start date");
```

### 3. Async Validation (Database Lookups)

```csharp
public class ProductValidator : AbstractValidator<ProductViewModel>
{
    private readonly IProductRepository _repository;

    public ProductValidator(IProductRepository repository)
    {
        _repository = repository;

        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MustAsync(async (name, cancellation) => 
                !await _repository.ExistsByNameAsync(name, cancellation))
            .WithMessage("Product name already exists");
    }
}
```

### 4. Custom Validators

```csharp
RuleFor(p => p.Sku)
    .Must(BeValidSkuFormat)
    .WithMessage("SKU must be in format XXX-NNNN");

private bool BeValidSkuFormat(string sku)
{
    return Regex.IsMatch(sku, @"^[A-Z]{3}-\d{4}$");
}
```

### 5. Cascade Modes

```csharp
RuleFor(p => p.Email)
    .Cascade(CascadeMode.Stop) // Stop on first error
    .NotEmpty()
    .EmailAddress()
    .MustAsync(BeUniqueEmail);
```

### 6. Rule Sets

```csharp
public class ProductValidator : AbstractValidator<ProductViewModel>
{
    public ProductValidator()
    {
        // Default rules
        RuleFor(p => p.Name).NotEmpty();
        
        // Create-specific rules
        RuleSet("Create", () =>
        {
            RuleFor(p => p.Id).Equal(0);
        });
        
        // Update-specific rules
        RuleSet("Update", () =>
        {
            RuleFor(p => p.Id).GreaterThan(0);
        });
    }
}

// Usage
await _validator.ValidateAsync(model, options => 
    options.IncludeRuleSets("Create"));
```

---

## 🧪 Testing Strategy

### Unit Test Structure

**File**: `tests/DemoApi.Application.Test/Validators/Products/ProductValidatorTests.cs`

```csharp
public class ProductValidatorTests
{
    private readonly ProductValidator _validator;

    public ProductValidatorTests()
    {
        _validator = new ProductValidator();
    }

    [Fact]
    public void Validate_EmptyName_ReturnsValidationError()
    {
        // Arrange
        ProductViewModel model = new() { Name = string.Empty, Weight = 1.0 };

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Name" && 
            e.ErrorMessage == "Name is required");
    }

    [Theory]
    [InlineData(-100)]
    [InlineData(-10.5)]
    [InlineData(0)]
    public void Validate_InvalidWeight_ReturnsValidationError(double weight)
    {
        // Arrange
        ProductViewModel model = new() { Name = "Test", Weight = weight };

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Weight");
    }
}
```

### Test Coverage

**Validators Layer**:
- ✅ **28 unit tests** (100% coverage)
- ✅ Empty/null/whitespace scenarios
- ✅ Boundary conditions (double.Epsilon, double.MaxValue)
- ✅ Edge cases (Unicode, special characters)
- ✅ Multiple validation failures
- ✅ Metadata verification

### Testing Best Practices

```csharp
// ✅ Test individual rules
[Fact]
public void Should_Validate_Name_NotEmpty() { }

// ✅ Test boundary conditions
[Fact]
public void Should_Accept_VerySmallPositiveWeight() { }

// ✅ Test multiple errors
[Fact]
public void Should_Return_Multiple_Errors_When_AllFieldsInvalid() { }

// ✅ Test edge cases
[Fact]
public void Should_Accept_UnicodeCharacters_In_Name() { }
```

---

## 📊 Validation Rules Reference

### Common Built-in Validators

| Validator | Example | Error Message |
|-----------|---------|---------------|
| `NotEmpty()` | `RuleFor(x => x.Name).NotEmpty()` | "{PropertyName} must not be empty" |
| `NotNull()` | `RuleFor(x => x.Model).NotNull()` | "{PropertyName} must not be null" |
| `Length(min, max)` | `RuleFor(x => x.Name).Length(3, 50)` | "{PropertyName} must be between 3 and 50 characters" |
| `EmailAddress()` | `RuleFor(x => x.Email).EmailAddress()` | "{PropertyName} is not a valid email" |
| `GreaterThan(value)` | `RuleFor(x => x.Weight).GreaterThan(0)` | "{PropertyName} must be greater than 0" |
| `LessThanOrEqualTo(value)` | `RuleFor(x => x.Price).LessThanOrEqualTo(1000)` | "{PropertyName} must be <= 1000" |
| `Matches(regex)` | `RuleFor(x => x.Sku).Matches("^[A-Z]{3}")` | "{PropertyName} is not in correct format" |
| `Must(predicate)` | `RuleFor(x => x.Code).Must(BeValidCode)` | Custom message |
| `MustAsync(predicate)` | `RuleFor(x => x.Name).MustAsync(BeUnique)` | Custom message |

### Custom Message Placeholders

```csharp
RuleFor(p => p.Name)
    .Length(3, 50)
    .WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");
```

---

## 🎯 Integration with ASP.NET Core

### Automatic Validation

**Before controller action**:
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] ProductViewModel product)
{
    // FluentValidation runs automatically here
    // If invalid, ModelState.IsValid = false
    
    // No need for manual validation
    var result = await _service.CreateAsync(product);
    return Ok(result);
}
```

### ModelValidationFilter

**File**: `src/DemoApi.Api/Extensions/ModelValidationFilter.cs`

```csharp
public class ModelValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            context.Result = new ObjectResult(new ResponseViewModel
            {
                Success = false,
                Data = null,
                Errors = errors
            })
            {
                StatusCode = StatusCodes.Status412PreconditionFailed
            };
        }
    }
}
```

---

## 🚀 Performance Considerations

### Benchmarks

| Validation Type | Latency | Notes |
|----------------|---------|-------|
| **Data Annotations** | ~0.05ms | Slightly faster |
| **FluentValidation** | ~0.1ms | Negligible overhead |
| **Async Validation** | ~5-50ms | Depends on DB query |

### Optimization Tips

1. **Avoid Over-Validation**: Only validate necessary fields
2. **Use Cascade Modes**: Stop on first error when appropriate
3. **Cache Validators**: Singleton/scoped lifetime (avoid transient)
4. **Minimize Async Calls**: Batch database lookups
5. **Rule Sets**: Separate create/update validations

---

## 📚 Best Practices

### ✅ Do

- ✅ Keep validators focused (single responsibility)
- ✅ Use explicit error messages (business-friendly)
- ✅ Test validators in isolation (unit tests)
- ✅ Leverage inheritance for shared rules
- ✅ Use async validation for database checks
- ✅ Implement rule sets for different scenarios
- ✅ Document complex validation logic

### ❌ Don't

- ❌ Mix business logic with validation
- ❌ Use validators for authorization checks
- ❌ Overuse async validation (performance impact)
- ❌ Ignore error message quality
- ❌ Duplicate validation rules across validators
- ❌ Validate inside constructors

---

## 📖 References

- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [ASP.NET Core Integration](https://docs.fluentvalidation.net/en/latest/aspnet.html)
- [Built-in Validators](https://docs.fluentvalidation.net/en/latest/built-in-validators.html)
- [Custom Validators](https://docs.fluentvalidation.net/en/latest/custom-validators.html)
- [Testing Validators](https://docs.fluentvalidation.net/en/latest/testing.html)

---
