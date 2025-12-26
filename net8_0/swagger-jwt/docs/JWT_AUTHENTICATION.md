# JWT Authentication Strategy

## 📋 Overview

This document details the **JWT (JSON Web Token)** authentication strategy implemented in DemoApi, including token generation, validation, and security considerations.

---

## 🔑 Token Generation

### Flow Diagram

```
Client Request
    ↓
Security Key Header Validation
    ↓
JWT Token Creation
    ↓
Token Signing (HS256)
    ↓
Structured Response (AccessToken + Metadata)
```

### Implementation

**File**: `src/DemoApi.Api/V1/Controllers/AuthController.cs`

```csharp
[AllowAnonymous]
[HttpPost("token")]
public IActionResult GenerateToken([FromHeader(Name = "X-Security-Key")] string securityKey)
{
    // 1. Defensive validation
    if (string.IsNullOrWhiteSpace(securityKey))
        return CustomResponse(HttpStatusCode.Unauthorized, null);

    // 2. Security key verification
    if (securityKey != _authorizationSettings.SecurityKey)
        return CustomResponse(HttpStatusCode.Unauthorized, null);

    // 3. Token descriptor creation
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Issuer = _authorizationSettings.Sender,
        Audience = _authorizationSettings.ValidOn,
        NotBefore = DateTime.UtcNow,
        Expires = DateTime.UtcNow.AddMinutes(_authorizationSettings.ExpirationMinutes),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        )
    };

    // 4. Token generation and serialization
    var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
    var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

    return CustomResponse(new TokenViewModel
    {
        AccessToken = encodedToken,
        TokenType = "Bearer",
        ExpiresIn = _authorizationSettings.ExpirationMinutes * 60,
        Created = created.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        Expires = expires.ToString("yyyy-MM-ddTHH:mm:ssZ")
    });
}
```

---

## ⚙️ Configuration

### JWT Settings

**File**: `src/DemoApi.Api/Configuration/JwtConfig.cs`

```csharp
public static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
{
    var authorization = configuration.GetSection("Authorization").Get<AuthorizationSettings>();

    // Defensive validation
    if (authorization is null)
        throw new InvalidOperationException("JWT settings missing");
    
    if (string.IsNullOrWhiteSpace(authorization.SecurityKey))
        throw new InvalidOperationException("SecurityKey required");
    
    if (authorization.SecurityKey.Length < 32)
        throw new InvalidOperationException("SecurityKey must be ≥32 chars");

    // JWT Bearer authentication setup
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // Strict expiration
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidAudience = authorization.ValidOn,
                ValidIssuer = authorization.Sender
            };
        });

    return services;
}
```

### Configuration File

**appsettings.json**:
```json
{
  "Authorization": {
    "SecurityKey": "minimum-32-character-secret-key-here",
    "Sender": "DemoApi",
    "ValidOn": "https://localhost:5001",
    "ExpirationMinutes": 60
  }
}
```

---

## 🔒 Token Validation

### Middleware Pipeline

```
Request
  ↓
ExceptionMiddleware
  ↓
UseAuthentication() ← JWT Validation happens here
  ↓
UseAuthorization()
  ↓
Controller [Authorize]
  ↓
Action Method
```

### Validation Parameters

| Parameter | Value | Purpose |
|-----------|-------|---------|
| **ValidateIssuer** | `true` | Ensures token was issued by trusted source |
| **ValidateAudience** | `true` | Ensures token is for this API |
| **ValidateIssuerSigningKey** | `true` | Verifies signature integrity |
| **ValidateLifetime** | `true` | Checks token expiration |
| **ClockSkew** | `TimeSpan.Zero` | No tolerance for expired tokens |

---

## 🎯 Usage Examples

### 1. Generate Token

**Request**:
```http
POST /api/v1/auth/token HTTP/1.1
Host: localhost:5001
X-Security-Key: your-32-character-minimum-secret-key
```

**Response**:
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJEZW1vQXBpIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsIm5iZiI6MTcwNTMyMDAwMCwiZXhwIjoxNzA1MzIzNjAwfQ.signature",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "created": "2024-01-15T10:00:00Z",
    "expires": "2024-01-15T11:00:00Z"
  }
}
```

### 2. Use Token

**Request**:
```http
GET /api/v1/products HTTP/1.1
Host: localhost:5001
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 🛡️ Security Measures

### 1. Key Length Enforcement

```csharp
if (authorization.SecurityKey.Length < 32)
{
    throw new InvalidOperationException(
        $"JWT SecurityKey must be at least 32 characters long. " +
        $"Current length: {authorization.SecurityKey.Length}"
    );
}
```

**Rationale**: Ensures minimum 256-bit key strength (HMAC-SHA256 requirement).

### 2. HTTPS Enforcement

```csharp
options.RequireHttpsMetadata = true;
```

**Rationale**: Prevents man-in-the-middle attacks by rejecting non-HTTPS metadata.

### 3. Zero Clock Skew

```csharp
ClockSkew = TimeSpan.Zero
```

**Rationale**: Default 5-minute tolerance removed for strict expiration enforcement.

### 4. Token Caching

```csharp
options.SaveToken = true;
```

**Rationale**: Caches validated token in HttpContext, reducing redundant validations.

---

## 📊 Token Structure

### JWT Anatomy

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9  ← Header (Base64)
.
eyJpc3MiOiJEZW1vQXBpIiwiYXVkIjoi...    ← Payload (Base64)
.
signature_hash_here                      ← Signature (HMAC-SHA256)
```

### Header
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

### Payload (Claims)
```json
{
  "iss": "DemoApi",                        // Issuer
  "aud": "https://localhost:5001",        // Audience
  "nbf": 1705320000,                      // Not Before (Unix timestamp)
  "exp": 1705323600                       // Expiration (Unix timestamp)
}
```

### Signature
```
HMACSHA256(
  base64UrlEncode(header) + "." + base64UrlEncode(payload),
  secret
)
```

---

## ⚡ Performance Characteristics

| Operation | Latency | Frequency |
|-----------|---------|-----------|
| **Token Generation** | ~2-5ms | Once per session (60 min default) |
| **Token Validation** | ~0.5-1ms | Every authenticated request |
| **Signature Verification** | ~0.3-0.5ms | Part of validation |

**Total Overhead**: +1-2ms per authenticated request (acceptable trade-off for security).

---

## 🔄 Refresh Token Pattern (Future)

### Current Limitation
- Tokens valid until expiration (no revocation)
- Users re-authenticate every 60 minutes

### Planned Enhancement
```csharp
public class RefreshTokenRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

[HttpPost("refresh")]
public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
{
    // 1. Validate refresh token
    // 2. Generate new access token
    // 3. Optionally rotate refresh token
    // 4. Return new token pair
}
```

---

## 🚨 Common Issues & Solutions

### Issue 1: 401 Unauthorized (Valid Token)

**Cause**: `ClockSkew = TimeSpan.Zero` with slight time differences.

**Solution**: Synchronize server time (NTP) or allow small skew:
```csharp
ClockSkew = TimeSpan.FromSeconds(5)
```

### Issue 2: Token Too Large

**Cause**: Excessive claims in payload.

**Solution**: Minimize claims, store additional data server-side.

### Issue 3: Token Stolen

**Cause**: Insecure client-side storage (localStorage).

**Solution**: 
- Use `httpOnly` cookies
- Implement short expiration
- Add refresh token rotation

---

## 📚 Best Practices

### ✅ Do

- ✅ Store `SecurityKey` in Azure Key Vault / AWS Secrets Manager
- ✅ Use environment-specific configurations
- ✅ Rotate keys periodically (implement versioning)
- ✅ Log authentication failures (without exposing keys)
- ✅ Use HTTPS in production (`RequireHttpsMetadata = true`)
- ✅ Implement rate limiting on `/auth/token`

### ❌ Don't

- ❌ Store `SecurityKey` in source control
- ❌ Use weak keys (<32 characters)
- ❌ Increase `ClockSkew` excessively (security risk)
- ❌ Store tokens in localStorage (XSS vulnerability)
- ❌ Disable HTTPS validation in production
- ❌ Log full tokens (sensitive data exposure)

---

## 🧪 Testing Strategy

### Unit Tests

```csharp
[Fact]
public void JwtConfig_ShouldThrowException_WhenSecurityKeyTooShort()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["Authorization:SecurityKey"] = "short-key" // <32 chars
        })
        .Build();

    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => 
        services.AddJwtConfig(config)
    );
}
```

### Integration Tests

```csharp
[Fact]
public async Task GenerateToken_ShouldReturnValidJwt_WhenSecurityKeyIsCorrect()
{
    // Arrange
    var client = _factory.CreateClient();
    client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

    // Act
    var response = await client.PostAsync("/api/v1/auth/token", null);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenViewModel>();
    tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
    tokenResponse.TokenType.Should().Be("Bearer");
}
```

---

## 🔮 Future Enhancements

1. **Refresh Tokens** (Q2 2024)
   - Long-lived sessions without security compromise
   - Token rotation on refresh

2. **Token Revocation** (Q2 2024)
   - Redis-based blacklist
   - Immediate invalidation capability

3. **RS256 Migration** (Q3 2024)
   - Asymmetric keys for multi-service architectures
   - Public/private key pairs

4. **Claims-Based Authorization** (Q2 2024)
   - Role claims (`[Authorize(Roles="Admin")]`)
   - Custom claims for fine-grained permissions

---

## 📖 References

- [RFC 7519 - JSON Web Token](https://datatracker.ietf.org/doc/html/rfc7519)
- [JWT.io - Debugger & Libraries](https://jwt.io/)
- [Microsoft Docs - JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt)
- [OWASP - JWT Security Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)

---
