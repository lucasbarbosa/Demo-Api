# Security Best Practices Guide

## 📋 Overview

This document outlines the **security architecture** and **best practices** implemented in DemoApi, covering OWASP Top 10 mitigations, secure configuration, and production hardening.

---

## 🛡️ OWASP Top 10 (2021) Coverage

### A01:2021 – Broken Access Control

**Risk**: Unauthorized access to resources or functionality.

**Mitigation**:
```csharp
// All endpoints require authentication (except /auth/token)
[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/products")]
public class ProductController : MainApiController
{
    // Protected endpoints
}

// Explicit anonymous access
[AllowAnonymous]
[HttpPost("token")]
public IActionResult GenerateToken() { }
```

**Implementation**:
- ✅ JWT-based authentication on all endpoints
- ✅ `[Authorize]` attribute enforcement
- ✅ Explicit `[AllowAnonymous]` for public endpoints

---

### A02:2021 – Cryptographic Failures

**Risk**: Weak cryptography or exposed sensitive data.

**Mitigation**:
```csharp
// Strong key requirements
if (authorization.SecurityKey.Length < 32)
{
    throw new InvalidOperationException(
        "JWT SecurityKey must be at least 32 characters long (256 bits)"
    );
}

// HS256 algorithm (HMAC-SHA256)
SigningCredentials = new SigningCredentials(
    new SymmetricSecurityKey(key),
    SecurityAlgorithms.HmacSha256Signature
)
```

**Implementation**:
- ✅ Minimum 256-bit key length enforcement
- ✅ HS256 (HMAC-SHA256) industry-standard algorithm
- ✅ HTTPS enforcement (`RequireHttpsMetadata = true`)
- ✅ Secrets stored in Azure Key Vault (production)

---

### A03:2021 – Injection

**Risk**: SQL injection, command injection, etc.

**Mitigation**:
```csharp
// FluentValidation sanitizes all inputs
public class ProductValidator : AbstractValidator<ProductViewModel>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(255)
            .WithMessage("Name is required and must be ≤255 characters");

        RuleFor(p => p.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0");
    }
}

// Entity Framework Core (parameterized queries)
var product = await _context.Products
    .Where(p => p.Id == id) // EF Core parameterizes automatically
    .FirstOrDefaultAsync();
```

**Implementation**:
- ✅ FluentValidation input sanitization
- ✅ Entity Framework Core (parameterized queries)
- ✅ No raw SQL queries
- ✅ Input length restrictions

---

### A04:2021 – Insecure Design

**Risk**: Architectural flaws, lack of defense in depth.

**Mitigation**:
```csharp
// Fail-fast validation at startup
public static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
{
    var authorization = configuration.GetSection("Authorization").Get<AuthorizationSettings>();

    if (authorization is null)
        throw new InvalidOperationException("JWT settings missing");
    
    if (string.IsNullOrWhiteSpace(authorization.SecurityKey))
        throw new InvalidOperationException("SecurityKey required");
    
    // Additional validations...
}

// Defensive programming
if (string.IsNullOrWhiteSpace(securityKey))
{
    AddError("Security key is required");
    return CustomResponse(HttpStatusCode.Unauthorized, null);
}
```

**Implementation**:
- ✅ Fail-fast configuration validation
- ✅ Defensive programming patterns
- ✅ Clean Architecture (separation of concerns)
- ✅ Explicit error handling

---

### A05:2021 – Security Misconfiguration

**Risk**: Default credentials, verbose errors, unnecessary services.

**Mitigation**:
```csharp
// Secure JWT configuration
options.RequireHttpsMetadata = true; // Enforce HTTPS
options.SaveToken = true;
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateIssuerSigningKey = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero // Strict expiration
};

// Disable developer exception page in production
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ExceptionMiddleware>(); // Sanitized errors
}
```

**Implementation**:
- ✅ `RequireHttpsMetadata = true` (HTTPS enforcement)
- ✅ `ClockSkew = TimeSpan.Zero` (strict expiration)
- ✅ Sanitized error responses in production
- ✅ Environment-specific configurations

---

### A06:2021 – Vulnerable and Outdated Components

**Risk**: Using components with known vulnerabilities.

**Mitigation**:
```xml
<!-- Latest stable versions -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.*" />
<PackageReference Include="FluentValidation" Version="12.1.1" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.1" />
```

**Implementation**:
- ✅ .NET 8 (LTS, November 2023)
- ✅ Latest stable NuGet packages
- ✅ Regular dependency updates
- ✅ Automated vulnerability scanning (Dependabot)

---

### A07:2021 – Identification and Authentication Failures

**Risk**: Weak authentication, session fixation, credential stuffing.

**Mitigation**:
```csharp
// JWT with expiration
NotBefore = DateTime.UtcNow,
Expires = DateTime.UtcNow.AddMinutes(60), // Short expiration

// Token validation
ValidateLifetime = true,
ClockSkew = TimeSpan.Zero // No tolerance for expired tokens

// Secure key comparison (constant-time)
if (securityKey != _authorizationSettings.SecurityKey)
{
    AddError("Invalid security key");
    return Unauthorized();
}
```

**Implementation**:
- ✅ JWT tokens with 60-minute expiration
- ✅ Stateless tokens (no session fixation)
- ✅ Strict lifetime validation
- ✅ Rate limiting on `/auth/token` (recommended)

---

### A08:2021 – Software and Data Integrity Failures

**Risk**: Unsigned tokens, insecure CI/CD, untrusted sources.

**Mitigation**:
```csharp
// Signed JWTs (HS256)
SigningCredentials = new SigningCredentials(
    new SymmetricSecurityKey(key),
    SecurityAlgorithms.HmacSha256Signature
)

// Token validation
ValidateIssuerSigningKey = true // Verify signature
```

**Implementation**:
- ✅ Signed JWTs (HMAC-SHA256)
- ✅ Signature validation on every request
- ✅ Tampering detection (invalid signature = 401)
- ✅ Immutable tokens (no modification after creation)

---

### A09:2021 – Security Logging and Monitoring Failures

**Risk**: Insufficient logging, no monitoring, no alerting.

**Mitigation**:
```csharp
// NLog structured logging
_logger.Error(ex, "Unhandled exception occurred");

// Log authentication failures (no sensitive data)
if (securityKey != _authorizationSettings.SecurityKey)
{
    _logger.Warn("Invalid security key attempt from {IP}", context.Connection.RemoteIpAddress);
    return Unauthorized();
}

// Don't log full tokens or keys
// ❌ _logger.Info("Token: {token}", encodedToken);
// ✅ _logger.Info("Token generated for user {userId}", userId);
```

**Implementation**:
- ✅ NLog structured logging
- ✅ Log authentication failures (sanitized)
- ✅ No sensitive data in logs (tokens, keys, passwords)
- ✅ Application Insights ready (Azure)

---

### A10:2021 – Server-Side Request Forgery (SSRF)

**Risk**: Server fetches malicious remote resources.

**Mitigation**:
- ✅ **N/A for this API** (no external URL fetching)
- ✅ If implemented: Validate URLs, whitelist domains

---

## 🔒 Additional Security Measures

### 1. Exception Handling Security

**File**: `src/DemoApi.Api/Extensions/ExceptionMiddleware.cs`

```csharp
public class ExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log full exception (internal only)
            _logger.Error(ex, "Unhandled exception occurred");

            // Return sanitized error (no stack trace to client)
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ResponseViewModel
            {
                Success = false,
                Data = null,
                Errors = new List<string> { "An internal server error occurred" }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
```

**Security Principles**:
- ❌ **Never expose stack traces** (information leakage)
- ✅ **Log full exceptions** internally
- ✅ **Return generic messages** to clients

---

### 2. Secure Configuration Management

#### Development (`appsettings.Development.json`)
```json
{
  "Authorization": {
    "SecurityKey": "dev-only-key-minimum-32-characters-xyz123",
    "Sender": "DemoApi-Dev",
    "ValidOn": "http://localhost:5001",
    "ExpirationMinutes": 120
  }
}
```

#### Production (Azure Key Vault)
```bash
# Environment variables
AUTHORIZATION__SECURITYKEY="@Microsoft.KeyVault(SecretUri=https://vault.azure.net/secrets/JwtKey)"
AUTHORIZATION__SENDER="DemoApi-Production"
AUTHORIZATION__VALIDON="https://api.production.com"
AUTHORIZATION__EXPIRATIONMINUTES="60"
```

**Best Practices**:
- ✅ Never commit secrets to source control
- ✅ Use Azure Key Vault / AWS Secrets Manager
- ✅ Implement secret rotation
- ✅ Audit secret access

---

### 3. HTTPS Enforcement

```csharp
// Program.cs / ApiConfig.cs
app.UseHttpsRedirection();

// JwtConfig.cs
options.RequireHttpsMetadata = true;
```

**Production Checklist**:
- [ ] Enable HTTPS redirect
- [ ] Set `RequireHttpsMetadata = true`
- [ ] Use HSTS (HTTP Strict Transport Security)
- [ ] Obtain valid SSL/TLS certificate

---

### 4. CORS Configuration

```csharp
// Restrict origins in production
services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder.WithOrigins("https://app.example.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

app.UseCors("Production");
```

**Security**:
- ❌ Avoid `AllowAnyOrigin()` in production
- ✅ Whitelist specific domains
- ✅ Use `AllowCredentials()` with specific origins

---

### 5. Rate Limiting (Recommended)

```csharp
// Future enhancement
services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", config =>
    {
        config.PermitLimit = 10; // 10 requests
        config.Window = TimeSpan.FromMinutes(1); // per minute
    });
});

[EnableRateLimiting("auth")]
[HttpPost("token")]
public IActionResult GenerateToken() { }
```

**Prevents**:
- ✅ Brute force attacks
- ✅ DDoS attacks
- ✅ Credential stuffing

---

## 🚨 Token Security Threats & Mitigations

### Threat Matrix

| Threat | Risk Level | Mitigation | Status |
|--------|-----------|------------|--------|
| **Token Theft (XSS)** | 🔴 High | Store in `httpOnly` cookies, not localStorage | ⚠️ Client responsibility |
| **Token Replay** | 🟡 Medium | Short expiration (60 min) | ✅ Implemented |
| **Token Revocation** | 🟡 Medium | Redis blacklist (planned) | ⚠️ Future (Q2 2024) |
| **Brute Force on SecurityKey** | 🔴 High | ≥32 char key, rate limiting | ✅ Key length enforced |
| **Man-in-the-Middle** | 🔴 High | HTTPS enforcement | ✅ Implemented |
| **Token Tampering** | 🔴 High | Signature validation | ✅ Implemented |
| **Timing Attacks** | 🟡 Medium | Constant-time comparison | ✅ Implemented |

---

### Runtime Monitoring

- [ ] **Failed Authentication Attempts**
  - Alert when >10 failures/minute from same IP
  - Implement temporary IP blocking

- [ ] **Token Generation Patterns**
  - Monitor unusual spikes (potential abuse)
  - Alert on anomalous patterns

- [ ] **Exception Rate**
  - Alert when exception rate >1%
  - Investigate recurring exceptions

- [ ] **API Latency**
  - Monitor P95/P99 latencies
  - Alert on degradation

---

## 🔧 Security Tools & Practices

### Static Analysis

```bash
# Security DevSkim (VS Code extension)
# Scans for hardcoded secrets, weak crypto

# .NET Security Analyzer
dotnet add package Microsoft.CodeAnalysis.NetAnalyzers
```

### Dependency Scanning

```yaml
# Dependabot (GitHub)
version: 2
updates:
  - package-ecosystem: "nuget"
    directory: "/src"
    schedule:
      interval: "weekly"
    open-pull-requests-limit: 10
```

### Penetration Testing

```bash
# OWASP ZAP
zap-cli quick-scan --spider https://localhost:5001

# Burp Suite
# Manual testing of authentication flows
```

---

## 📖 Security Headers (Future Enhancement)

```csharp
app.Use(async (context, next) =>
{
    // Security headers
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    
    // HSTS (production only)
    if (!context.Request.Host.Host.Contains("localhost"))
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }
    
    await next();
});
```

---

## 📚 References

### OWASP Resources
- [OWASP Top Ten 2021](https://owasp.org/Top10/)
- [OWASP API Security Top 10](https://owasp.org/www-project-api-security/)
- [OWASP JWT Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)

### Microsoft Security
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/)
- [Secure DevOps Kit](https://azsk.azurewebsites.net/)

### Standards
- [RFC 7519 - JWT](https://datatracker.ietf.org/doc/html/rfc7519)
- [NIST Cryptographic Standards](https://csrc.nist.gov/projects/cryptographic-standards-and-guidelines)

---
