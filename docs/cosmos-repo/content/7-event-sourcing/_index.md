---
title: "Event Sourcing"
weight: 7
chapter: true
pre: "<b>7. </b>"
---

# Event Sourcing

Cosmos is a great choice for a data store when implementing event sourcing. The `IEvangelist.Azure.CosmosEventSourcing` package provides a set of features built on top of `IEvangelist.Azure.CosmosRepository` that make this experience very enjoyable.

{{% notice warning %}}
This package is currently in pre-release. A major release is due soon, this means the API surface may change from now until then.
{{% /notice %}}

## Shipping Sample App

The best place to go to see the library is the sample project. Under the `/samples` directory you can find the example project. This shows how to setup and use cosmos DB as an event store. See a snippet from this sample below.

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddSwaggerGen();
services.AddEndpointsApiExplorer();

services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(options =>
    {
        options.DatabaseId = "event-sourcing-shipping-sample";
        options.ContainerBuilder
            .ConfigureProjectionStore<ShipInformation>("ship-projections")
            .ConfigureEventSourceStore<ShipEventSource>("ship-tracking-events");
    });

    eventSourcingBuilder.AddAllPersistedEventsTypes();
    eventSourcingBuilder.AddAllEventProjectionHandlers();
    eventSourcingBuilder.AddEventBasedEventSourceProjectionBuilder<ShipEventSource>(options =>
    {
        options.ProcessorName = "shipping-demo";
        options.InstanceName = Environment.MachineName;
    });
});

services.AddCosmosRepositoryChangeFeedHostedService();

var app = builder.Build();
app.Run();
```