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

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCosmosRepository(Configuration);
    }
    ```

    The optional `setupAction` allows consumers to manually configure the `RepositoryOptions` object

    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCosmosRepository(Configuration,
            options => 
            {
                options.CosmosConnectionString = "< connection string >";
                options.ContainerId = "data-store";
                options.DatabaseId = "samples";
            });
    }
    ```

1. Define your object graph, must inherit `Item`

    ```csharp
    using Microsoft.Azure.CosmosRepository;

    public class Person : Item
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    ```

1. Ask for an instance of `IRepository<TItem>`

    ```csharp
    using Microsoft.Azure.CosmosRepository;

    public class Consumer
    {
        readonly IRepository<Person> _repository;

        public Consumer(IRepository<Person> repository) =>
            _respository = repository;

        // Use the repo...
    }
    ```

1. Enjoy!