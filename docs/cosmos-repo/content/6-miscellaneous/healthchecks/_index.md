---
title: "Health Checks"
weight: 2
---

The `IEvangelist.Azure.CosmosRepository.AspNetCore` package adds support for [AspNet Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) using the [HealthChecks.CosmosDb](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.CosmosDb/README.md) package.

## Setup

To configure Cosmos DB health checks:

```csharp
services.AddHealthChecks().AddCosmosRepository();
```

By default, this will scan all of the assemblies in your solution to locate the container names for each of your `IItem` types.  To refine this, and potentially reduce startup times, pass in the Assemblies containing your `IItem` types:

```csharp
services.AddHealthChecks().AddCosmosRepository(assemblies: typeof(ExampleItem).Assembly);
```

Alternatively, use the override to provide your own `AzureCosmosDbHealthCheckOptions` and configure the Containers to check:

```csharp
services.AddHealthChecks().AddCosmosRepository(optionsFactory: sp => new AzureCosmosDbHealthCheckOptions
{
    DatabaseId = "my-database",
    ContainerIds = new[] { "Container1", "Container2" }
});
```

Use `optionsFactory: null` to retain the default behaviour of the [HealthChecks.CosmosDb](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.CosmosDb/README.md) package, and only call `CosmosClient.ReadAccountAsync()`.

```csharp
services.AddHealthChecks().AddCosmosRepository(optionsFactory: null);
```

The Cosmos Repository Health package supports all of the existing functionality of Health Checks, such as failureStatus and tags, see the [Microsoft documentation](https://learn.microsoft.com/aspnet/core/host-and-deploy/health-checks) for configuration details.

Don't forget to map the health endpoint with:

```csharp
app.MapHealthChecks("/healthz");
```
