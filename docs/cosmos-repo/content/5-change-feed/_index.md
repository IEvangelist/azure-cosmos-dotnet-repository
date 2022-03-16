---
title: "Change Feed"
weight: 5
chapter: true
pre: "<b>5. </b>"
---

# Change Feed Processing

## Overview

Cosmos DB provides a mechanism for listening to changes in a container. This includes when an item is created or updated. This can be used for many scenarios such as:

1. Creating events when items update.
1. Replicating data into another Cosmos DB container. Perhaps with a different partitioning strategy.
1. Replicating data into another storage medium, possibly SQL server.

{{% notice info %}}
The change feed does not currently support tracking the deletion of items. However, there are way you can do this with a soft delete. A soft delete can be done by setting a short TTL on the item.
{{% /notice %}}

## Listening to changes for an `IItem`

It is possible to listen for changes on any `IItem` you have a container configured for in this library. 

#### Configuring


To start off you need to instruct the library that you would like to changes in a container for a given `IItem`. This is done via the `IItemContainerBuilder` which you can gain access to adding the repository into the IoC container. An example of this is shown below.

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository(options =>
{
    options.DatabaseId = "change-feed-demo";
    options.ContainerPerItemType = true;

    options.ContainerBuilder.Configure<Book>(containerOptions =>
    {
        containerOptions.WithContainer(BookDemoConstants.Container);
        containerOptions.WithPartitionKey(BookDemoConstants.PartitionKey);

        //Listen to to the change feed
        containerOptions.WithChangeFeedMonitoring();
    });
}
```

#### Change Feed Processors


In order to process changes you need define a class that will process changes for a given `IItem`. This class is known as a processor and it _must_ implement the `IItemChangeFeedProcessor<TItem>` interface. An example of a class implementing this interface is shown below.

```csharp
public class BookChangeFeedProcessor : IItemChangeFeedProcessor<Book>
{
    private readonly ILogger<BookChangeFeedProcessor> _logger;
    private readonly IRepository<BookByIdReference> _bookByIdReferenceRepository;

    public BookChangeFeedProcessor(ILogger<BookChangeFeedProcessor> logger,
        IRepository<BookByIdReference> bookByIdReferenceRepository)
    {
        _logger = logger;
        _bookByIdReferenceRepository = bookByIdReferenceRepository;
    }

    public async ValueTask HandleAsync(Book rating, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Change detected for book with ID: {BookId}", rating.Id);

        if (!rating.HasBeenUpdated)
        {
            await _bookByIdReferenceRepository
                .CreateAsync(new BookByIdReference(rating.Id, rating.Category),
                cancellationToken);
        }

        _logger.LogInformation("Processed change for book with ID: {BookId}", rating.Id);
    }
}
```

