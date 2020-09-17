[![NuGet](https://img.shields.io/nuget/v/IEvangelist.Azure.CosmosRepository.svg?style=flat)](https://www.nuget.org/packages/IEvangelist.Azure.CosmosRepository/)

# IEvangelist Azure CosmosDB Repository .NET SDK

This package wraps the [NuGet: Microsoft.Azure.Cosmos package](https://www.nuget.org/packages/Microsoft.Azure.Cosmos), 
exposing a simple dependecny-injection enabled `IRepository<T>` interface. The repository is responsible for all 
of the create, read, update, and delete (CRUD) operations on objects `where T : Document`. The `Document` type adds 
a single property which is a globaly unique identifier defined as:

```csharp
[JsonProperty("id")]
public string Id { get; set; } = Guid.NewGuid().ToString();
```