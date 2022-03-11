---
title: "Partitioning"
weight: 1
---

Partitioning is a huge part of Cosmos DB. There are many different strategies you may want to take. Such as, splitting different types of item's across containers, sharing some `IItem`'s in one container and not others and finally, in some cases, even putting all `IItem` types in a single container.

## Default Strategy

Adding the Azure Cosmos Repository with no real configuration, like below. Will result in a simple strategy being assumed on your behalf. This will leave the option's configured in there default state. This means a property called `ContainerPerItemType` is set to false. This means that the Azure Cosmos Repository will store _all_ `IItem`'s in the same container, setting all the `IItem`'s within to use the partition key `/id`.

```csharp
using Microsoft.Azure.CosmosRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository();

var app = builder.Build();

app.MapGet("/", () => 
    "Default Cosmos Repository Partitioning Strategy");

app.Run();
```

The default partitioning strategy is simple, all `IItem`'s will be in a single logical partition split by there unique IDs. Which default to a GUID. This in small applications is often ample. A simple rule of thumb for whether or not this would be okay for you would be, do you see yourself spanning over a physical partition? If no then this option _could_ be the way to go. However, in any scenarios where you are making an application that may grow in size, complexity or scale. It may be best to look at taking a bit more control over your partitioning strategy.

> This strategy is also demonstrated in the [`WebTier`](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/samples/WebTier) sample application.

## Taking Control

The next configuration allows you to take more control over the partitioning strategy. The first thing to do is to set the `ContainerPerItemType` equal to `true`. This can be done like in the example below. Once this field has been set, then you need to define which container each `IItem` should be placed into and also what partition key it should use. There are currently 2 methods to do this in the Azure Cosmos Repository.

```csharp
using Microsoft.Azure.CosmosRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository(options => 
{
    options.ContainerPerItemType = true;
});

var app = builder.Build();

app.MapGet("/", () => 
    "Custom Cosmos Repository Partitioning Strategy");

app.Run();
```

### The `IItem` Container Builder

The first option that you have to configure `IItem`'s container options is to use the `IItemContainerBuilder` which you can access via the `RepositoryOptions`. See the examples below.

#### Customer Order System Example

In the below example we have two `IItem` types defined. One is the `Customer` item which is configured to be stored in the `customers` container, partitioned by `/emailAddress`. The second is the `Order` item, partitioned by `/customerId`.

```csharp
using Microsoft.Azure.CosmosRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository(options => 
{
    options.ContainerPerItemType = true;

    options.ContainerBuilder.Configure<Customer>(userContainerOptions => 
    {    
        userContainerOptions.WithContainer("customer");
        userContainerOptions.WithPartitionKey("/emailAddress");
    });

    options.ContainerBuilder.Configure<Order>(orderContainerOptions => 
    {    
        orderContainerOptions.WithContainer("orders");
        orderContainerOptions.WithPartitionKey("/customerId");
    });
});

var app = builder.Build();

app.MapGet("/", () => 
    "Sample e-commerce Partitioning Strategy");

app.Run();
```

#### Inventory Example

The below example shows how two or more `IItem` types can share a container and a partitioning strategy. This defines a `Stock` item that might hold the name, price, size, and maybe weight. This is partitioned by the `/stockReferenceNumber`. The `StockRecord` item is a collection of `StockRecord`'s. This record represents when a piece of stock was added or removed from the inventory, when this occurred, and maybe a reason why. This is stored in the _same_ container as the `Stock` item whose history it is recording. It also shares the same partitioning strategy. In the example below it means that all `Stock` and its `StockRecord`'s are stored in the same logical partition in Cosmos DB.
 
```csharp
using Microsoft.Azure.CosmosRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository(options => 
{
    options.ContainerPerItemType = true;

    options.ContainerBuilder.Configure<Stock>(userContainerOptions => 
    {    
        userContainerOptions.WithContainer("stock");
        userContainerOptions.WithPartitionKey("/stockReferenceNumber");
    });

    options.ContainerBuilder.Configure<StockRecord>(orderContainerOptions => 
    {    
        orderContainerOptions.WithContainer("stock");
        orderContainerOptions.WithPartitionKey("/stockReferenceNumber");
    });
});

var app = builder.Build();

app.MapGet("/", () => 
    "Sample Inventory Partitioning Strategy");

app.Run();
```

### Attribute usage

Another option to configure a partitioning strategy is to use attributes to decorate your `IItem` types. See the examples below.

#### `Customer` Partitioning Strategy

The example below show's how you can achieve a custom partitioning strategy using attributes.

```csharp
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository;

[Container("customers")]
[PartitionKeyPath("/emailAddress")]
public class Customer : FullItem
{
    public string EmailAddress { get; set; }

    protected override string GetPartitionKeyValue() => 
        EmailAddress; 
}
```

> Note the `override` of the `GetPartitionKeyValue` method. This is required to tell the library which property to get your partition key value from.