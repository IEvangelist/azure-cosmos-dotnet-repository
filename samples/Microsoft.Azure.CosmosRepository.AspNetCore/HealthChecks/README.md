# Health Checks Sample

This sample demonstrates the **eager container initialization** feature for Cosmos DB health checks.

## What It Demonstrates

- Using `EagerlyInitializeCosmosContainersAsync()` to pre-create containers at startup
- Configuring health checks with `AddCosmosRepository()`
- Ensuring containers exist before health check endpoints become available
- Preventing health check failures during lazy container initialization

## Key Features

### Eager Initialization

The sample calls `await app.Services.EagerlyInitializeCosmosContainersAsync()` **after `Build()` but before `Run()`**:

```csharp
var app = builder.Build();

// Containers are created here, before the app starts
await app.Services.EagerlyInitializeCosmosContainersAsync();

app.Run(); // Health checks only available after containers exist
```

### Health Check Configuration

Health checks are configured with a simple extension method:

```csharp
builder.Services
    .AddHealthChecks()
    .AddCosmosRepository();
```

## Running the Sample

### Prerequisites

- Cosmos DB Emulator running locally, or update the connection string in `appsettings.json`

### Steps

1. Start the Cosmos DB Emulator (or configure a real Cosmos DB connection)
2. Run the application:
   ```bash
   dotnet run
   ```
3. Check the logs to see eager initialization:
   ```
   Eagerly initializing 1 Cosmos DB container(s) for 1 item type(s)...
   Successfully initialized container 'products' for item type 'Product'
   Successfully completed eager initialization of 1 Cosmos DB container(s)
   ```
4. Navigate to `/health` to verify health checks pass immediately
5. Use Swagger UI at `/swagger` to test the API endpoints

## Testing Different Scenarios

### Scenario 1: With Eager Initialization (Default)

Leave the code as-is. Health checks will pass immediately on startup.

### Scenario 2: Without Eager Initialization

Comment out the eager initialization line:

```csharp
// await app.Services.EagerlyInitializeCosmosContainersAsync();
```

The first health check may fail if containers haven't been created yet through repository operations.

### Scenario 3: Auto-Creation Disabled

Set `IsAutoResourceCreationIfNotExistsEnabled` to `false` in `appsettings.json`. The eager initialization will be skipped (logged as debug), and containers must already exist.

## Endpoints

- `GET /health` - Health check endpoint
- `GET /api/products` - List all products
- `POST /api/products` - Create a new product
- `GET /swagger` - Swagger UI

## Database Created

When you run this sample with `IsAutoResourceCreationIfNotExistsEnabled = true`, you'll see:

- **Database**: `health-check-demo`
- **Container**: `products` with partition key `/category`
