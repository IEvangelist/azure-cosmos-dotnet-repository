# Projections

Projections allow you to build up an alternate view of your events, notify system's outside your domains boundary of this event and more! This is done by processing all new events in a separate process much like you would process events off of a queue.

This is fully supported in the library by making using of a feature of Cosmos DB called the change feed. The change feed is a persistent log of writes and updates to any items in a container. This means whenever we write a new event, this will be picked up by the change feed. We can then setup a background worker to process these changes.

Projections can be used as a read store to show current state of an `AggregateRoot` _without_ having to replay all the events. For `AggregateRoot`'s that have a lot of events this can often be a good fit. You could also take the opportunity to take this internal event to your application and offer that to other systems by taking this event and placing it on a message broker for example.

## Basic Projections

The most basic form of a projection is to build another model from the events you are writing to the `EventStore<TEventItem>`. This model may be a quick snapshot of the current state of your events, or it could be an aggregation of those events into a more meaningful data structure to meet a business requirement. A great example of this is detailed [here.](../getting-started/guide/04-read-projection.md) It can also perform another action in your system later in time. This is great for scenarios where an operation needs to occur but it's not critical it happens as part of the same request that say assigns a customer's address. This example show's how that can be done.

:::tip Tip
If you haven't already followed the [Getting Started - Guide](../getting-started/guide/00-overview.md), it may be best to give that a read, as this example follow's on from it.
:::

Taking the example of the `CustomerAccount` let's say that you wanted to send a welcome letter once a customer has provided there address details. This can be done by processing that addition of the `CustomerAccountAddressAssigned` event. Let's see an example implementation of an `IEventItemProjection<TEventItem, TProjectionKey>` below.

```csharp
using EventSourcingCustomerAccount.Events;
using EventSourcingCustomerAccount.Items;
using EventSourcingCustomerAccount.Models;
using EventSourcingCustomerAccount.Services;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingCustomerAccount.Projections;

public record WelcomeLetterProjectionKey : IProjectionKey;

public class WelcomeLetterProjection :
    IEventItemProjection<CustomerAccountEventItem, WelcomeLetterProjectionKey>
{
    private readonly IReadOnlyRepository<CustomerAccountReadItem> _repository;
    private readonly IPostalService _postalService;

    public WelcomeLetterProjection(
        IReadOnlyRepository<CustomerAccountReadItem> repository,
        IPostalService postalService)
    {
        _repository = repository;
        _postalService = postalService;
    }

    public async ValueTask ProjectAsync(
        CustomerAccountEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        if (eventItem.DomainEvent is CustomerAccountAddressAssigned addressAssigned)
        {
            CustomerAccountReadItem? account = await _repository.TryGetAsync(
                addressAssigned.Username,
                cancellationToken: cancellationToken);

            if (account is not null)
            {
                await _postalService.SendWelcomeLetterAsync(
                    account.FirstName,
                    account.Surname,
                    new CustomerAddress(
                        addressAssigned.AddressLine1,
                        addressAssigned.AddressLine2,
                        addressAssigned.City,
                        addressAssigned.Country,
                        addressAssigned.PostCode));
            }
        }
    }
}
```

This example simply processes an events that are of the type `CustomerAccountAddressAssigned` and uses an `IPostalService` to send a welcome letter. This happens in it's own process triggered via a background service when the event is written.

Take not of the `WelcomeLetterProjectionKey` record at the top of the file.

```csharp
public record WelcomeLetterProjectionKey : IProjectionKey;
```

This is used by the library to provide a separate processor running against the change feed for each projection you want to build. Think of this much like multiple subscribers from a topic, a single event is written to the topic and it can span out to many consumers. The same works with change feed processors. These are then added to the library with a name that describes the projection that is being built or the action that is being performed. See below:

```csharp
builder.Services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(options =>
    {
        options.DatabaseId = "customer-accounts-sample-db";
        options.ContainerBuilder
            .ConfigureEventItemStore<CustomerAccountEventItem>(
                "customer-account-events")
            .ConfigureProjectionStore<CustomerAccountReadItem>(
                containerName: "projections",
                partitionKey: "/username");
    });

    // Other config excluded for brevity

    eventSourcingBuilder
        .AddEventItemProjection<CustomerAccountEventItem,
            WelcomeLetterProjectionKey,
            WelcomeLetterProjection>(
            options =>
            {
                options.ProcessorName = "welcome-letter-builder";
                options.InstanceName = Environment.MachineName;
            });
});

// Provided by the IEvangelist.CosmosRepository.AspNetCore nuget package.
builder.Services.AddCosmosRepositoryChangeFeedHostedService();
```

:::tip Tip
The `WelcomeLetterProjectionKey` is used in the registration process and is tied directly to the processor with the name `welcome-letter-builder`. This enables the library to resolve the correct services and start a fresh processor per projection.
:::

There are two keys point to take not of here. The fist is the processor name, this is used by the Cosmos DB change feed processor library to identify this processor. Let's say we had an application that was scaled horizontally to 3 instances. They all declare a processor with the same name `welcome-letter-builder`. The library will then distributed all changes across those 3 instances, effectively load balancing changes for you. The second property is the instance name, this should reflect the node or compute instance that the app is running on. In this example we use the machine name, but this could be a pod id in Kubernetes, or a node name on an app service.

## Handling Failures

It is vitally important that exceptions thrown when processing an event from the change feed is handled properly by the consumer. Depending on the criticality of the role your projection is playing determines how you need to handle the failure of processing an event. The change feed processor library will retry changes infinitely that result in un-handled exceptions. This makes it even more important that non-transient errors handled and logged for manual intervention in the future.

The library also is extremely careful when handling errors in it's part of the changes pipeline. The main place the library has to be careful is when deserializing it's events. The library will return a special event type in the case of a deserialization failure. The `NonDeserializableEvent` can be handled by a consumer projections and can provide relevant information, such as the payload as a JObject, the exception that caused the failure, or whether or not the types where just not registered.