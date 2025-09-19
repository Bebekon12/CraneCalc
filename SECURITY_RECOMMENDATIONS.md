# Рекомендации по исправлению проблем безопасности

## 1. Исправление экспозиции секретных ключей

### Текущая проблема
```json
// appsettings.json - НЕ БЕЗОПАСНО
{
  "JwtOptions": {
    "SecretKey": "ahdahudhahdoahwwohdauhdoahdoaudhoudhoawhdouhawodawhdaod"
  },
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5439;Database=CartCalc;User Id=Bebekon;Password=123;"
  }
}
```

### Рекомендуемое решение
```json
// appsettings.json - БЕЗОПАСНО
{
  "JwtOptions": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "ExpiresHours": "${JWT_EXPIRES_HOURS:12}"
  },
  "ConnectionStrings": {
    "Postgres": "${DATABASE_CONNECTION_STRING}"
  }
}
```

```csharp
// Program.cs - добавить поддержку переменных окружения
var builder = WebApplication.CreateBuilder(args);

// Добавить переменные окружения с приоритетом
builder.Configuration.AddEnvironmentVariables();

// Валидация обязательных переменных
var requiredEnvVars = new[] { "JWT_SECRET_KEY", "DATABASE_CONNECTION_STRING" };
foreach (var envVar in requiredEnvVars)
{
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envVar)))
    {
        throw new InvalidOperationException($"Required environment variable {envVar} is not set");
    }
}
```

## 2. Улучшение конфигурации JWT

### Текущая проблема
```csharp
// AuthConfig.cs - НЕ БЕЗОПАСНО
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
};
```

### Рекомендуемое решение
```csharp
// AuthConfig.cs - БЕЗОПАСНО
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = jwtOptions.Issuer,
    ValidAudience = jwtOptions.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
    ClockSkew = TimeSpan.FromMinutes(5)
};
```

```csharp
// JwtOptions.cs - расширенная конфигурация
public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresHours { get; set; }
    public int AccessTokenExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
```

## 3. Безопасные cookies

### Текущая проблема
```csharp
// UserController.cs - НЕ БЕЗОПАСНО
Response.Cookies.Append(CookieNames.AccessToken, result.AccessToken, new CookieOptions
{
    HttpOnly = true,
    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
});
```

### Рекомендуемое решение
```csharp
// UserController.cs - БЕЗОПАСНО
Response.Cookies.Append(CookieNames.AccessToken, result.AccessToken, new CookieOptions
{
    HttpOnly = true,
    Secure = true, // Только HTTPS
    SameSite = SameSiteMode.Strict, // CSRF protection
    Expires = DateTimeOffset.UtcNow.AddMinutes(15),
    Path = "/api" // Ограничить область действия
});

Response.Cookies.Append(CookieNames.RefreshToken, result.RefreshToken, new CookieOptions
{
    HttpOnly = true,
    Secure = true,
    SameSite = SameSiteMode.Strict,
    Expires = DateTimeOffset.UtcNow.AddDays(7),
    Path = "/api/user" // Ещё более ограниченная область
});
```

## 4. Устранение хардкода администратора

### Текущая проблема
```csharp
// JwtProvider.cs - НЕ БЕЗОПАСНО
new(ClaimTypes.Role, userModel.Role == Role.Moderator || userModel.Login=="bebekon" ? "Administrator" : "User")
```

### Рекомендуемое решение
```csharp
// Добавить enum для ролей
public enum Role
{
    User = 0,
    Moderator = 1,
    Administrator = 2
}

// JwtProvider.cs - БЕЗОПАСНО
public string GenerateToken(UserModel userModel)
{
    Claim[] claims = [
        new(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
        new(ClaimTypes.Name, userModel.Login),
        new(ClaimTypes.Role, userModel.Role.ToString())
    ];
    
    // Добавить дополнительные права если нужно
    if (userModel.Role == Role.Administrator)
    {
        claims = claims.Append(new Claim("permission", "admin")).ToArray();
    }
    
    // ... остальная логика
}
```

## 5. Rate Limiting

```csharp
// Program.cs - добавить rate limiting
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: token);
    };
});

var app = builder.Build();

app.UseRateLimiter();
```

```csharp
// UserController.cs - применить rate limiting для аутентификации
[EnableRateLimiting("AuthPolicy")]
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken ct)
{
    // ... логика логина
}

// Добавить в конфигурацию специальную политику для аутентификации
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("AuthPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(15) // 5 попыток в 15 минут
            }));
});
```

## 6. Улучшенная валидация паролей

```csharp
// RegisterCommandValidator.cs - улучшенная валидация
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин не должен быть пустым")
            .MinimumLength(3).WithMessage("Логин должен содержать минимум 3 символа")
            .MaximumLength(50).WithMessage("Логин должен содержать не более 50 символов")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Логин может содержать только буквы, цифры и подчеркивания");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не должен быть пустым")
            .MinimumLength(8).WithMessage("Пароль должен содержать минимум 8 символов")
            .MaximumLength(100).WithMessage("Пароль должен содержать не более 100 символов")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Пароль должен содержать минимум одну строчную букву, одну заглавную букву, одну цифру и один специальный символ");
    }
}
```

## 7. Структурированное логирование

```csharp
// RefreshTokenFilter.cs - улучшенная обработка ошибок
public class RefreshTokenFilter(
    IJwtProvider jwtProvider,
    ITokenStorage tokenStorage,
    IUserRepository userRepository,
    ILogger<RefreshTokenFilter> logger) // Добавить logger
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            // ... логика фильтра
        }
        catch (SecurityTokenException ex)
        {
            logger.LogWarning(ex, "Invalid token in refresh filter for user {UserId}", 
                GetUserIdFromRequest(context));
            context.Result = new UnauthorizedResult();
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in RefreshTokenFilter for request {RequestPath}", 
                context.HttpContext.Request.Path);
            context.Result = new StatusCodeResult(500);
            return;
        }

        await next();
    }
}
```

## 8. Кэширование пользователей

```csharp
// UserRepository.cs - добавить кэширование
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UserRepository> _logger;
    
    public async Task<UserModel?> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        var cacheKey = $"user:{id}";
        
        if (_cache.TryGetValue(cacheKey, out UserModel? cachedUser))
        {
            return cachedUser;
        }
        
        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
            
        if (entity == null)
            return null;
            
        var user = entity.ToModel();
        
        // Кэшировать на 15 минут
        _cache.Set(cacheKey, user, TimeSpan.FromMinutes(15));
        
        return user;
    }
}
```

## 9. Graceful Shutdown

```csharp
// Program.cs - добавить graceful shutdown
var app = builder.Build();

// Настройка graceful shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStopping.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application is shutting down gracefully...");
    
    // Дать время завершить текущие запросы
    Thread.Sleep(TimeSpan.FromSeconds(5));
});

app.Run();
```

## 10. Конфигурация для разных окружений

### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "CraneCalc": "Debug"
    }
  },
  "JwtOptions": {
    "ExpiresHours": "1"
  }
}
```

### appsettings.Production.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtOptions": {
    "ExpiresHours": "12"
  }
}
```

## Заключение

Эти рекомендации помогут значительно улучшить безопасность приложения. Важно внедрять изменения поэтапно, начиная с критических проблем безопасности, а затем переходя к улучшениям архитектуры и производительности.