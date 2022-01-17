# Partitioning Setup

Partitioning is a huge part of Cosmos DB. There are many different strategies you may want to take. Such as, splitting different types of item's across containers, sharing some item's in one container and not others and finally, in some cases, even putting all item types in a single container.

## Default Strategy

Adding the Azure Cosmos Repository with no real configuration, like below. Will result in a simple strategy being assumed on your behalf. This will leave the option's configured in there default state. This means a property called `ContainerPerItemType` is set to false. This means that the Azure Cosmos Repository will store _all_ items in the same container, setting all the item's within to use the partition key `/id`.

```c#
using Microsoft.Azure.CosmosRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository();

var app = builder.Build();

app.MapGet("/", () => "Default Cosmos Repository Partitioning Strategy");

app.Run();
```

The default partitioning strategy is simple all items will be in a single logical partition split by there unique IDs, which default to a GUID. This in small applications is often ample. A simple rule of thumb for this would be do you see yourself spanning over a physical partition? If no then this option _could_ be the way to go. However, in any scenarios where you are making an application that may grow in size, complexity or scale. It may be best to look at taking a bit more control over your partitioning strategy.

> This strategy is also demonstrated in the [`WebTier`](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/samples/WebTier) sample application.

## Taking Control

The next configuration allows you to take more control over the partitioning strategy 