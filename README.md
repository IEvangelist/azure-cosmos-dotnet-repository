[![NuGet](https://img.shields.io/nuget/v/IEvangelist.Azure.CosmosRepository.svg?style=flat)](https://www.nuget.org/packages/IEvangelist.Azure.CosmosRepository/)

# Azure Cosmos DB Repository .NET SDK

This package wraps the [NuGet: Microsoft.Azure.Cosmos package](https://www.nuget.org/packages/Microsoft.Azure.Cosmos), 
exposing a simple dependency-injection enabled `IRepository<T>` interface. The repository is responsible for all 
of the create, read, update, and delete (CRUD) operations on objects `where T : Item`. The `Item` type adds 
a single property which is a globally unique identifier defined as:

```csharp
[JsonProperty("id")]
public string Id { get; set; } = Guid.NewGuid().ToString();
```

## Getting started

1. Create an Azure Cosmos DB SQL resource
1. Obtain the resource connection string from the __Keys__ blade
1. Call `AddCosmosRepository` and provide the apps configuration object
    - The optional `setupAction` allows consumers to manually configure the `RepositoryOptions` object
1. Ask for an instance of `IRepository<T>`
1. Enjoy!