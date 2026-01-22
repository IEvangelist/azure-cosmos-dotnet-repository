---
title: "Container Initialization"
weight: 4
---

This page explains how and when Cosmos DB containers are created when using the repository pattern.

## Lazy Initialization (Default)

By default, Cosmos DB containers are created **lazily** - they are created on the first repository operation that accesses them.

```csharp
// Container doesn't exist yet
var repository = serviceProvider.GetRequiredService<IRepository<Product>>();

// Container is created here on first access
var product = await repository.CreateAsync(new Product { Name = "Widget" });
```

### How Lazy Initialization Works

1. Application starts - no containers are created
2. First repository operation (`CreateAsync`, `GetAsync`, etc.) is called
3. The `ICosmosContainerService` checks if the container exists
4. If `IsAutoResourceCreationIfNotExistsEnabled` is `true`, the container is created
5. The repository operation completes

### Configuration

Lazy initialization is controlled by the `IsAutoResourceCreationIfNotExistsEnabled` option:

```csharp
builder.Services.AddCosmosRepository(options =>
{
    options.IsAutoResourceCreationIfNotExistsEnabled = true; // Default
});
```

**When `IsAutoResourceCreationIfNotExistsEnabled` is `true`:**
- Containers are created automatically on first access
- Database is created if it doesn't exist
- Convenient for development and environments where you control the infrastructure

**When `IsAutoResourceCreationIfNotExistsEnabled` is `false`:**
- Containers must already exist
- Application throws an exception if a container is missing
- Recommended for production when using Infrastructure as Code (Terraform, ARM templates, etc.)

### Pros and Cons of Lazy Initialization

✅ **Pros:**
- Simple - no additional code required
- Containers only created if actually used
- Good for development

❌ **Cons:**
- First request has higher latency (container creation time)
- Health checks may fail during startup while containers are being created
- Configuration errors not discovered until runtime
- Can cause issues with Kubernetes readiness probes

## Eager Initialization

Eager initialization creates containers **at application startup** before any requests are handled.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository(options =>
{
    options.ContainerBuilder.Configure<Product>(c => 
        c.WithContainer("products").WithPartitionKey("/category"));
});

var app = builder.Build();

// Eagerly initialize containers before the app starts
await app.Services.EagerlyInitializeCosmosContainersAsync();

app.Run();
```

### How Eager Initialization Works

The `EagerlyInitializeCosmosContainersAsync()` method:

1. Checks if `IsAutoResourceCreationIfNotExistsEnabled` is `true` (returns immediately if `false`)
2. Discovers item types in priority order:
   - Types explicitly configured via `ContainerBuilder.Configure<TItem>()`
   - Types from provided assemblies (if assemblies parameter is provided)
   - Types from all loaded assemblies (as fallback)
3. Calls `ICosmosContainerService.GetContainerAsync()` for each type
4. Waits for all containers to be created before returning
5. Logs detailed information about the initialization process

### When to Use Eager Initialization

✅ **Use when:**
- Your application uses health checks (Kubernetes, load balancers, etc.)
- You want consistent performance on all requests (including the first)
- You want to fail fast during startup if Cosmos DB is misconfigured
- Running in containerized environments with readiness probes
- You have a small number of containers

❌ **Don't use when:**
- `IsAutoResourceCreationIfNotExistsEnabled` is `false` (it does nothing)
- You have many containers and want faster startup time
- Containers are created through Infrastructure as Code
- You're okay with lazy initialization behavior

### Explicit Type Discovery

If you have item types but they're not being discovered automatically, you can pass assemblies explicitly:

```csharp
await app.Services.EagerlyInitializeCosmosContainersAsync(
    typeof(Product).Assembly,
    typeof(Order).Assembly);
```

However, if you use `ContainerBuilder.Configure<T>()`, types are discovered automatically:

```csharp
// Product will be discovered automatically - no assembly parameter needed
builder.Services.AddCosmosRepository(options =>
{
    options.ContainerBuilder.Configure<Product>(c => c.WithContainer("products"));
});

await app.Services.EagerlyInitializeCosmosContainersAsync(); // Discovers Product
```

## Comparison

| Aspect | Lazy Initialization | Eager Initialization |
|--------|-------------------|---------------------|
| **When containers are created** | On first repository access | At application startup |
| **First request latency** | Higher (includes container creation) | Normal (containers already exist) |
| **Startup time** | Faster | Slower (waits for containers) |
| **Configuration** | Default behavior | Requires explicit call |
| **Health checks** | May fail during initialization | Pass immediately after startup |
| **Fail-fast behavior** | Errors discovered at runtime | Errors discovered at startup |
| **Best for** | Development, simple apps | Production with health checks |

## Common Scenarios

### Development Environment

```csharp
// Simple setup - lazy initialization is fine
builder.Services.AddCosmosRepository(options =>
{
    options.CosmosConnectionString = "AccountEndpoint=https://localhost:8081/...";
    options.IsAutoResourceCreationIfNotExistsEnabled = true;
});
```

### Production with Health Checks

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository(options =>
{
    options.IsAutoResourceCreationIfNotExistsEnabled = true;
    options.ContainerBuilder.Configure<Product>(c => c.WithContainer("products"));
    options.ContainerBuilder.Configure<Order>(c => c.WithContainer("orders"));
});

builder.Services.AddHealthChecks().AddCosmosRepository();

var app = builder.Build();

// Ensure containers exist before health checks run
await app.Services.EagerlyInitializeCosmosContainersAsync();

app.MapHealthChecks("/health");
app.Run();
```

### Production with Infrastructure as Code

```csharp
// Containers created externally (Terraform, ARM templates, etc.)
builder.Services.AddCosmosRepository(options =>
{
    options.IsAutoResourceCreationIfNotExistsEnabled = false; // Containers must exist
});

// No eager initialization needed - containers already exist
```

## Logging

Both initialization approaches provide detailed logging:

```
// Lazy initialization
[Debug] Container 'products' does not exist. Creating...
[Information] Successfully created container 'products'

// Eager initialization
[Information] Eagerly initializing 2 Cosmos DB container(s) for 2 item type(s)
[Debug] Successfully initialized container 'products' for item type 'Product'
[Debug] Successfully initialized container 'orders' for item type 'Order'
[Information] Successfully completed eager initialization of 2 Cosmos DB container(s)
```

Set logging level to `Debug` to see detailed initialization information:

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.Azure.CosmosRepository": "Debug"
    }
  }
}
```

## Best Practices

1. **Use eager initialization with health checks** - Prevents health check failures during container creation
2. **Configure types explicitly** - Use `ContainerBuilder.Configure<T>()` for automatic discovery
3. **Set `IsAutoResourceCreationIfNotExistsEnabled = false` in production** - When using Infrastructure as Code
4. **Monitor startup logs** - Verify containers are created successfully
5. **Use appropriate timeouts** - For Kubernetes startup probes when using eager initialization
