# Анализ производительности и архитектуры CraneCalc

## Резюме производительности

Система CraneCalc демонстрирует базовую функциональность, но имеет несколько узких мест в производительности и архитектурных решениях, которые могут повлиять на масштабируемость.

## 🚀 Проблемы производительности

### 1. Отсутствие пула соединений Redis

**Файл:** `CraneCalc.API/Configurations/RedisConfig.cs`
**Проблема:** Конфигурация Redis может не оптимально использовать соединения

**Рекомендация:**
```csharp
public static void AddRedisConfig(this IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("Redis");
    
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = connectionString;
        options.ConfigurationOptions = new ConfigurationOptions
        {
            EndPoints = { connectionString.Split(',')[0] },
            Password = "redispassword", // Из переменных окружения
            ConnectRetry = 3,
            ConnectTimeout = 5000,
            SyncTimeout = 5000,
            KeepAlive = 180,
            DefaultDatabase = 0,
            Ssl = false, // true для продакшена
            AbortOnConnectFail = false
        };
    });
}
```

### 2. N+1 Проблема в загрузке данных

**Файл:** `CraneCalc.API/Filters/RefreshTokenFilter.cs`
**Проблема:** Каждый запрос делает отдельный запрос к БД для получения пользователя

**Рекомендуемое решение:**
```csharp
public class CachedUserRepository : IUserRepository
{
    private readonly IUserRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedUserRepository> _logger;
    
    public async Task<UserModel?> GetUserByIdAsync(Guid id, CancellationToken ct)
    {
        var cacheKey = $"user_by_id:{id}";
        
        if (_cache.TryGetValue(cacheKey, out UserModel? cachedUser))
        {
            _logger.LogDebug("User {UserId} found in cache", id);
            return cachedUser;
        }
        
        var user = await _innerRepository.GetUserByIdAsync(id, ct);
        
        if (user != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                SlidingExpiration = TimeSpan.FromMinutes(5),
                Priority = CacheItemPriority.High
            };
            
            _cache.Set(cacheKey, user, cacheOptions);
            _logger.LogDebug("User {UserId} cached for 15 minutes", id);
        }
        
        return user;
    }
}
```

### 3. Неэффективные Entity Framework запросы

**Файл:** `CraneCalc.Infrastructure/AppDbContext.cs`
**Проблема:** Отсутствие оптимизации запросов и индексов

**Рекомендации:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Оптимизированные индексы
    modelBuilder.Entity<UserEntity>()
        .HasIndex(u => u.Login)
        .IsUnique()
        .HasDatabaseName("IX_Users_Login");
    
    modelBuilder.Entity<CargoEntity>()
        .HasIndex(c => c.Type)
        .HasDatabaseName("IX_Cargo_Type");
        
    modelBuilder.Entity<CargoEntity>()
        .HasIndex(c => c.Weight)
        .HasDatabaseName("IX_Cargo_Weight");
    
    // Составной индекс для поиска
    modelBuilder.Entity<CargoEntity>()
        .HasIndex(c => new { c.Type, c.Weight })
        .HasDatabaseName("IX_Cargo_Type_Weight");
    
    base.OnModelCreating(modelBuilder);
}

// Добавить в репозиторий оптимизированные запросы
public async Task<List<CargoModel>> GetCargoByTypeAsync(string type, CancellationToken ct)
{
    return await _context.Cargos
        .AsNoTracking() // Важно для read-only операций
        .Where(c => c.Type == type)
        .OrderBy(c => c.Title)
        .Select(c => new CargoModel // Projection для снижения трафика
        {
            Id = c.Id,
            Title = c.Title,
            Type = c.Type,
            Weight = c.Weight
        })
        .ToListAsync(ct);
}
```

### 4. Отсутствие пагинации

**Файл:** `CraneCalc.Web/Controllers/HomeController.cs`
**Проблема:** Загрузка всех элементов без пагинации

**Рекомендуемое решение:**
```csharp
public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage => PageNumber * PageSize < TotalCount;
    public bool HasPreviousPage => PageNumber > 1;
}

[HttpGet]
public async Task<IActionResult> Services(
    [FromQuery] string? search, 
    [FromQuery] int page = 1, 
    [FromQuery] int pageSize = 10)
{
    var query = _context.Cargos.AsNoTracking();
    
    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(c => c.Type.Contains(search) || c.Title.Contains(search));
    }
    
    var totalCount = await query.CountAsync();
    
    var items = await query
        .OrderBy(c => c.Title)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    var result = new PaginatedResult<CargoModel>
    {
        Items = items.Select(c => c.ToModel()).ToList(),
        TotalCount = totalCount,
        PageNumber = page,
        PageSize = pageSize
    };
    
    return Ok(result);
}
```

## 🏗️ Архитектурные улучшения

### 5. Внедрение паттерна CQRS

**Текущая проблема:** Смешивание команд и запросов в одних сервисах

**Рекомендуемое решение:**
```csharp
// Queries
public class GetCargoByIdQuery : IRequest<CargoModel?>
{
    public Guid Id { get; set; }
}

public class GetCargoByIdQueryHandler : IRequestHandler<GetCargoByIdQuery, CargoModel?>
{
    private readonly ICargoReadRepository _readRepository;
    private readonly IMemoryCache _cache;
    
    public async Task<CargoModel?> Handle(GetCargoByIdQuery request, CancellationToken ct)
    {
        var cacheKey = $"cargo:{request.Id}";
        
        if (_cache.TryGetValue(cacheKey, out CargoModel? cachedCargo))
            return cachedCargo;
            
        var cargo = await _readRepository.GetByIdAsync(request.Id, ct);
        
        if (cargo != null)
        {
            _cache.Set(cacheKey, cargo, TimeSpan.FromMinutes(30));
        }
        
        return cargo;
    }
}

// Commands  
public class CreateCargoCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Weight { get; set; }
}

public class CreateCargoCommandHandler : IRequestHandler<CreateCargoCommand, Guid>
{
    private readonly ICargoWriteRepository _writeRepository;
    private readonly IMemoryCache _cache;
    
    public async Task<Guid> Handle(CreateCargoCommand request, CancellationToken ct)
    {
        var cargo = new CargoEntity
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Type = request.Type,
            Weight = request.Weight
        };
        
        await _writeRepository.AddAsync(cargo, ct);
        
        // Инвалидация кэша
        _cache.Remove($"cargo_list:{request.Type}");
        
        return cargo.Id;
    }
}
```

### 6. Улучшение обработки ошибок

**Текущая проблема:** Непоследовательная обработка ошибок

**Рекомендуемое решение:**
```csharp
// Создать middleware для обработки ошибок
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            EntityException => new { error = exception.Message, statusCode = 400 },
            UnauthorizedAccessException => new { error = "Unauthorized", statusCode = 401 },
            SecurityTokenException => new { error = "Invalid token", statusCode = 401 },
            _ => new { error = "Internal server error", statusCode = 500 }
        };

        context.Response.StatusCode = response.statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

### 7. Добавление мониторинга производительности

```csharp
// Program.cs - добавить мониторинг
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Добавить мониторинг
builder.Services.AddSingleton<DiagnosticSource>(new DiagnosticListener("CraneCalc"));

// Добавить health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// Добавить middleware для измерения времени выполнения
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();
    
    if (stopwatch.ElapsedMilliseconds > 1000) // Логировать медленные запросы
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Slow request: {Method} {Path} took {ElapsedMs}ms", 
            context.Request.Method, 
            context.Request.Path, 
            stopwatch.ElapsedMilliseconds);
    }
});

// Health check endpoint
app.MapHealthChecks("/health");
```

## 📊 Мониторинг и метрики

### 8. Добавление Application Insights или аналогов

```csharp
// Создать сервис для метрик
public interface IMetricsService
{
    void IncrementCounter(string name, params (string Key, string Value)[] tags);
    void RecordValue(string name, double value, params (string Key, string Value)[] tags);
    void RecordDuration(string name, TimeSpan duration, params (string Key, string Value)[] tags);
}

public class MetricsService : IMetricsService
{
    private readonly ILogger<MetricsService> _logger;
    
    public void IncrementCounter(string name, params (string Key, string Value)[] tags)
    {
        // Интеграция с системой мониторинга (Prometheus, AppInsights, etc.)
        _logger.LogInformation("Counter {CounterName} incremented with tags {Tags}", 
            name, string.Join(", ", tags.Select(t => $"{t.Key}={t.Value}")));
    }
    
    public void RecordValue(string name, double value, params (string Key, string Value)[] tags)
    {
        _logger.LogInformation("Metric {MetricName} recorded value {Value} with tags {Tags}", 
            name, value, string.Join(", ", tags.Select(t => $"{t.Key}={t.Value}")));
    }
    
    public void RecordDuration(string name, TimeSpan duration, params (string Key, string Value)[] tags)
    {
        _logger.LogInformation("Duration {MetricName} recorded {Duration}ms with tags {Tags}", 
            name, duration.TotalMilliseconds, string.Join(", ", tags.Select(t => $"{t.Key}={t.Value}")));
    }
}

// Использование в контроллерах
[ApiController]
public class CargoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMetricsService _metrics;
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCargo(Guid id, CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var cargo = await _mediator.Send(new GetCargoByIdQuery { Id = id }, ct);
            
            _metrics.IncrementCounter("cargo.get.success", ("type", cargo?.Type ?? "unknown"));
            _metrics.RecordDuration("cargo.get.duration", stopwatch.Elapsed);
            
            return cargo == null ? NotFound() : Ok(cargo);
        }
        catch (Exception ex)
        {
            _metrics.IncrementCounter("cargo.get.error", ("error_type", ex.GetType().Name));
            throw;
        }
    }
}
```

## 🔧 Оптимизация базы данных

### 9. Стратегии кэширования

```csharp
// Многоуровневая стратегия кэширования
public class MultiLevelCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<MultiLevelCacheService> _logger;
    
    public async Task<T?> GetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null) where T : class
    {
        // Level 1: Memory cache
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            _logger.LogDebug("Cache hit (memory): {Key}", key);
            return cachedValue;
        }
        
        // Level 2: Distributed cache (Redis)
        var distributedValue = await _distributedCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(distributedValue))
        {
            var deserializedValue = JsonSerializer.Deserialize<T>(distributedValue);
            
            // Populate memory cache
            _memoryCache.Set(key, deserializedValue, TimeSpan.FromMinutes(5));
            
            _logger.LogDebug("Cache hit (distributed): {Key}", key);
            return deserializedValue;
        }
        
        // Level 3: Factory (database/API call)
        var value = await factory();
        if (value != null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            var cacheExpiry = expiry ?? TimeSpan.FromMinutes(30);
            
            // Set in both caches
            await _distributedCache.SetStringAsync(key, serializedValue, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheExpiry
            });
            
            _memoryCache.Set(key, value, TimeSpan.FromMinutes(Math.Min(5, cacheExpiry.TotalMinutes)));
            
            _logger.LogDebug("Cache miss, value cached: {Key}", key);
        }
        
        return value;
    }
}
```

### 10. Асинхронная обработка

```csharp
// Background service для тяжелых операций
public class CargoIndexingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CargoIndexingService> _logger;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var cargoService = scope.ServiceProvider.GetRequiredService<ICargoService>();
                
                // Переиндексация поисковых данных каждый час
                await cargoService.RebuildSearchIndexAsync(stoppingToken);
                
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cargo indexing service");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
```

## 📈 Заключение по производительности

### Текущее состояние
- ⚡ **Базовая функциональность:** Работает, но неоптимально
- 📊 **Масштабируемость:** Ограничена отсутствием кэширования и пагинации
- 🔍 **Мониторинг:** Практически отсутствует

### Приоритеты оптимизации

1. **Критичные (немедленно):**
   - Добавить кэширование пользователей
   - Внедрить пагинацию
   - Оптимизировать запросы к БД

2. **Высокий приоритет:**
   - Добавить мониторинг производительности
   - Реализовать graceful shutdown
   - Добавить health checks

3. **Средний приоритет:**
   - Внедрить CQRS паттерн
   - Добавить background services
   - Оптимизировать Redis конфигурацию

### Ожидаемые улучшения
- 🚀 **Скорость ответа:** Улучшение в 3-5 раз за счет кэширования
- 📈 **Пропускная способность:** Увеличение в 2-3 раза
- 🔧 **Поддерживаемость:** Значительное улучшение благодаря мониторингу
- ⚖️ **Масштабируемость:** Готовность к увеличению нагрузки в 10 раз