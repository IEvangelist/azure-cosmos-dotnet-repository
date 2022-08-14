# Persistence

The `IEventStore<TEventItem>` offers a few different ways to persist events into the store. Each method that you could choose to use does ultimately the same thing, stores a set of `EventItem`'s into the database. However, each method offers a different way to write these events following some different patterns that are worth explaining. 

:::tip Tip
If you want to use an interface which only support's write operations the library offers the [`IWriteOnlyEventStore<TEventItem>`](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/src/Microsoft.Azure.CosmosEventSourcing/Stores/IWriteOnlyEventStore.cs)
:::

## Events

The libraries offers an API which supports saving collections of `EventItem`'s as transactional batches (see the [ACID Transactions](#acid-transactions) section for more information).

See a simple example of using the `PersistAsync(...)` method below, this example reads all the events off of an `AggregateRoot` and saves them using the `IEventStore<TEventItem>`.

```csharp
public class ShipRepository : IShipRepository
{
    private readonly IEventStore<ShipEventItem> _store;

    public ShipRepository(IEventStore<ShipEventItem> store)
    {
        _store = store;
    }

    public ValueTask SaveAsync(Ship ship)
    {
        List<ShipEventItem> events = ship
            .NewEvents
            .Select(x =>
                new ShipEventItem(x, ship.Name))
            .ToList();

        events.Add(new ShipEventItem(ship.AtomicEvent, ship.Name));

        return _store.PersistAsync(events);
    }
}
```

## Aggregate Roots

```csharp
//TODO:
```

## ACID Transactions

The library supports full ACID transactions on events being persisted. It is a common scenario in event sourcing that you could have multiple consumers in parallel reading what they think is current state of an aggregate. They then apply one or many new events to there current picture of an aggregate. It is _important_ that when both of the consumers write back there new events that only one update wins. If they where to both write there events this can cause issues. One issue is that there would be multiple events with the same `Sequence` number. You would also have no guarantee that when you replay those events again that they would be valid when being applied to your aggregate.

In order to combat this complexity the library makes use of two features of Cosmos DB. The first is the [transaction batch feature.](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/transactional-batch) The second is the [optimistic concurrency control](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/database-transactions-optimistic-concurrency#optimistic-concurrency-control).

These two features of Cosmos DB work great together, the transactional batch ensures that either all insert/update operations succeed or fail as a batch. Optimistic concurrency control prevents lost updates and ensures only one concurrent insert/update will ever succeed. In order to implement optimistic concurrency control on a set of events the library always reads back and saves a special event. This is called the `AtomicEvent` every time you persist new events you must provide this event. This is also required by the libraries `AggregateRoot` implementation when replaying events. Whenever, you add a new event the time stamp on the `AtomicEvent` is updated. This will force it's record in the database to be assigned a new `etag` (used by Cosmos DB to implement optimistic concurrency control), if another consumer tries to write it's `AtomicEvent` back with a different `etag` value it will fail. This would fail the transactional batch operation. Resulting in _no_ new events being persisting.

If an update fails when persisting a new set of events the library throws a custom exception [`BatchOperationException`](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/src/Microsoft.Azure.CosmosRepository/Exceptions/BatchOperationException.cs). This contains the native [`TransactionalBatchResponse`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.transactionalbatchresponse?view=azure-dotnet) provided by the Azure Cosmos DB .NET SDK. A consumer can use this to inspect why the update operation failed.

As a consumer of this library there are a few ways to handle this exception gracefully:

1. If it was the result of a user request it may be reasonable to simply ask them to try again.
1. If the operation that is performing the update is coming from a message broker, then a lot of these implement the concept of abandoning a message. This can be used to re-process this operation again a few seconds later.
1. Use a [Polly](https://github.com/App-vNext/Polly) policy implementation to retry the full operation again.

:::warning Caution
It is _important_ in any of the above solutions that the full operation is retried. i.e. read the new state of the aggregate and re-apply the new events on the _new_ version of the aggregate.
:::

## Correlation

The event sourcing provider supports correlation IDs. These can be setup by consuming the scoped service `IContextService` and setting the `IContextService.CorrelationId` property. This will then be written into Cosmos DB for each event that was created in that context. This is also automatically populated when consuming changes from the change feed (projections).

See below for some example middleware that makes use of the popular `CorrelationId` package for ASPNET CORE, to set a correlation ID for every HTTP request.

```csharp
app.Use(async (context, next) =>
{
    ICorrelationContextAccessor correlationContextAccessor =
        context.RequestServices.GetRequiredService<ICorrelationContextAccessor>();
    
    IContextService contextService = context.RequestServices
        .GetRequiredService<IContextService>();

    contextService.CorrelationId = correlationContextAccessor.CorrelationContext.CorrelationId;

    await next.Invoke();
});
```