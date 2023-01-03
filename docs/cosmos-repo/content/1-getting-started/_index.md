---
title: "Getting Started"
weight: 1
chapter: true
pre: "<b>1. </b>"
---

The quickest way to get started using the `Microsoft.Azure.CosmosRepository` package is to use the `AddCosmosRepository` extension method on the `IServiceCollection` interface. This extension method will register the necessary services and configure the repository SDK for you.

1. Create an Azure Cosmos DB SQL resource.
1. Obtain the resource connection string from the **Keys** blade, be sure to get a connection string and not the key - these are different. The connection string is a compound key and endpoint URL.
1. Call `AddCosmosRepository`:

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddCosmosRepository();
   }
   ```

   The optional `setupAction` allows consumers to manually configure the `RepositoryOptions` object:

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddCosmosRepository(
           options =>
           {
               options.CosmosConnectionString = "< connection string >";
               options.ContainerId = "data-store";
               options.DatabaseId = "samples";
           });
   }
   ```

1. Define your object graph, objects must inherit `Item`, for example:

   ```csharp
   using Microsoft.Azure.CosmosRepository;

   public class Person : Item
   {
       public string FirstName { get; set; }
       public string LastName { get; set; }
   }
   ```

1. Ask for an instance of `IRepository<TItem>`, in this case the `TItem` is `Person`:

   ```csharp
   using Microsoft.Azure.CosmosRepository;

   public class Consumer
   {
       readonly IRepository<Person> _repository;

       public Consumer(IRepository<Person> repository) =>
           _repository = repository;

       // Use the repo...
   }
   ```

1. Perform any of the operations on the `_repository` instance, create `Person` records, update them, read them, or delete.
1. Enjoy!

## Configuration

When `OptimizeBandwidth` is `true` (its default value), the repository SDK reduces networking and
CPU load by not sending the resource back over the network and serializing it to the client. This is specific to writes,
such as create, update, and delete. For more information, see [Optimizing bandwidth in the Azure Cosmos DB .NET SDK](https://devblogs.microsoft.com/cosmosdb/enable-content-response-on-write).

There is much debate with how to structure your database and corresponding containers. Many developers with relational
database design experience might prefer to have a single container per item type, while others understand that Azure Cosmos DB
will handle things correctly regardless. By default, the `ContainerPerItemType` option is `false` and
all items are persisted into the same container. However, when it is `true`, each distinct subclass of
`Item` gets its own container named by the class itself.

### Well-known keys

Depending on the [.NET configuration provider](https://docs.microsoft.com/dotnet/core/extensions/configuration-providers?WC.m_id=dapine) your app is using, there are several well-known keys that map to the repository options that configure your usage of the repository SDK. When using environment variables, such as those in Azure App Service configuration or Azure Key Vault secrets, the following keys map to the `RepositoryOptions` instance:

| Key | Data type | Default value |
|---|---|---|
| RepositoryOptions\_\_CosmosConnectionString | string | `null` |
| RepositoryOptions\_\_AccountEndpoint | string | `null` |
| RepositoryOptions\_\_DatabaseId | string | `"database"` |
| RepositoryOptions\_\_ContainerId | string | `"container"` |
| RepositoryOptions\_\_OptimizeBandwidth | boolean | `true` |
| RepositoryOptions\_\_ContainerPerItemType | boolean | `false` |
| RepositoryOptions\_\_AllowBulkExecution | boolean | `false` |
| RepositoryOptions**SerializationOptions**IgnoreNullValues | boolean | `false` |
| RepositoryOptions**SerializationOptions**Indented | boolean | `false` |
| RepositoryOptions**SerializationOptions**PropertyNamingPolicy | CosmosPropertyNamingPolicy | `CosmosPropertyNamingPolicy.CamelCase` |

### Example `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "RepositoryOptions": {
    "CosmosConnectionString": "<Your-CosmosDB-ConnectionString>",
    "AccountEndpoint": "<Your-CosmosDB-URI>"
    "DatabaseId": "<Your-CosmosDB-DatabaseName>",
    "ContainerId": "<Your-CosmosDB-ContainerName>",
    "OptimizeBandwidth": true,
    "ContainerPerItemType": true,
    "AllowBulkExecution": true,
    "SerializationOptions": {
      "IgnoreNullValues": true,
     "PropertyNamingPolicy": "CamelCase"
    }
  }
}
```

For more information, see [JSON configuration provider](https://docs.microsoft.com/dotnet/core/extensions/configuration-providers?WC.m_id=dapine#json-configuration-provider).

### Example `appsettings.json` with Azure Functions

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Values": {
    "RepositoryOptions:CosmosConnectionString": "<Your-CosmosDB-ConnectionString>",
    "RepositoryOptions:AccountEndpoint": "<Your-CosmosDB-URI>",
    "RepositoryOptions:DatabaseId": "<Your-CosmosDB-DatabaseName>",
    "RepositoryOptions:ContainerId": "<Your-CosmosDB-ContainerName>",
    "RepositoryOptions:OptimizeBandwidth": true,
    "RepositoryOptions:ContainerPerItemType": true,
    "RepositoryOptions:AllowBulkExecution": true,
    "RepositoryOptions:SerializationOptions:IgnoreNullValues": true,
    "RepositoryOptions:SerializationOptions:PropertyNamingPolicy": "CamelCase"
  }
}
```

## Advanced partitioning strategy

As a consumer of Azure Cosmos DB, you can choose how to partition your data. By default, this repository SDK will partition items using their `Item.Id` value as the `/id` partition in the storage container. However, you can override this default behavior by:

1. Declaratively specifying the partition key path with `PartitionKeyPathAttribute`
1. Override the `Item.GetPartitionKeyValue()` method
1. Ensure the the property value of the composite or synthetic key is serialized to match the partition key path
1. Set `RepositoryOptions__ContainerPerItemType` to `true`, to ensure that your item with explicit partitioning is correctly maintained

As an example, considering the following:

```csharp
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;
using System;

namespace Example
{
    [PartitionKeyPath("/synthetic")]
    public class Person : Item
    {
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;

        [JsonProperty("synthetic")]
        public string SyntheticPartitionKey =>
            $"{FirstName}-{LastName}"; // Also known as a "composite key".

        protected override string GetPartitionKeyValue() => SyntheticPartitionKey;
    }
}
```
