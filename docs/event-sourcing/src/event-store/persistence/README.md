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

In the context of event sourcing, the Aggregate plays a crucial role in managing state changes. Here are its main responsibilities:

 1. *Track State Changes*: The Aggregate tracks changes to its state by recording a sequence of events. Each event represents a state change. Instead of storing the current state, the Aggregate stores these events. The current state is derived by replaying these events.

1. *Ensure Consistency*: The Aggregate ensures that any changes to its state are consistent. This means that the state of the Aggregate after an event is always valid according to the business rules of the domain.

1. *Handle Commands*: The Aggregate handles commands, which are requests to perform some action. A command may result in state changes, which are captured as events.

1. *Apply Events*: The Aggregate applies events to change its state. This is done in a deterministic way, meaning given the same sequence of events, the final state of the Aggregate will always be the same.

Remember, the key idea behind an Aggregate in event sourcing is that the state is not directly modified. Instead, changes are captured as events, and the state is derived from these events. This allows for powerful capabilities like temporal querying, event replay, and more.

### Organization

In C#, partial classes allow a single class to be split across multiple files. This can be particularly useful when working with Aggregate Roots in event sourcing for several reasons:

1. *Separation of Concerns*: By using partial classes, you can separate the different responsibilities of the Aggregate Root into different files. For example, you can have one file for handling commands, another for applying events, and another for defining the state. This makes it easier to understand what each part of the Aggregate Root is responsible for.

1. *Readability*: Large classes can be difficult to navigate and understand. By splitting the Aggregate Root into multiple files, you can make the code more readable. Each file can focus on a specific aspect of the Aggregate Root, making it easier to find and understand the relevant code.

1. *Maintainability*: As the complexity of the Aggregate Root grows, having it split into multiple files can make it easier to maintain. Changes to one aspect of the Aggregate Root are less likely to impact other aspects, reducing the risk of introducing bugs.

Remember, the Apply methods in the Aggregate Root are crucial as they are responsible for changing the state of the Aggregate. These methods must always succeed once applied. By separating these methods into their own file, you can highlight their importance and make it easier to ensure they are implemented correctly.

:::tip NOTICE
It is very important that there is no reason for an `Apply` method to fail. Once an event has been saved into the store it should be considered a fact. If this was to fail in the process of reading back the aggregate and re-applying the events, it would corrupt the state of the aggregate, making it un-readable.
:::

See below for an example of of splitting the `Ship` aggregate root.

#### Apply

```csharp
public partial class Ship
{
    private Ship() { }

    public static Ship Build(List<DomainEvent> persistedEvents)
    {
        Ship ship = new();
        ship.Apply(persistedEvents);
        return ship;
    }

    private void Apply(ShipEvents.ShipCreated shipCreated)
    {
        (var name, DateTime commissioned) = shipCreated;
        Name = name;
        Commissioned = commissioned;
    }

    private void Apply(ShipEvents.DockedInPort dockedInPort)
    {
        Port = dockedInPort.Port;
        Status = ShipStatus.Docked;
    }

    private void Apply(ShipEvents.Loading _) =>
        Status = ShipStatus.Loading;

    private void Apply(ShipEvents.Loaded loaded)
    {
        CargoWeight = loaded.CargoWeight;
        Status = ShipStatus.AwaitingDeparture;
    }

    private void Apply(ShipEvents.Departed _) =>
        Status = ShipStatus.AtSea;

    protected override void Apply(DomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ShipEvents.ShipCreated created:
                Apply(created);
                break;
            case ShipEvents.DockedInPort dockedInPort:
                Apply(dockedInPort);
                break;
            case ShipEvents.Loading loading:
                Apply(loading);
                break;
            case ShipEvents.Loaded loaded:
                Apply(loaded);
                break;
            case ShipEvents.Departed departed:
                Apply(departed);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(domainEvent),
                    $"No apply method found for {domainEvent.GetType().Name}");
        }
    }
}
```

#### Definition + behavior methods

```csharp
public partial class Ship : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public DateTime Commissioned { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public ShipStatus Status { get; private set; }
    public string? Port { get; private set; }
    public double? CargoWeight { get; private set; }

    public Ship(string name, DateTime commissioned, DateTime? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException<Ship>("A ship name must be provided");
        }

        AddEvent(new ShipEvents.ShipCreated(name, commissioned));
    }

    public void Dock(string port)
    {
        if (Status is not (ShipStatus.AtSea or ShipStatus.UnUsed))
        {
            throw new DomainException<Ship>($"A ship cannot dock when at status {Status}");
        }

        AddEvent(new ShipEvents.DockedInPort(Name, port));
    }

    public void StartLoading(string port)
    {
        if (Status is not (ShipStatus.Docked or ShipStatus.UnUsed))
        {
            throw new DomainException<Ship>($"A ship cannot start loading when at status {Status}");
        }

        if (Port != port)
        {
            throw new DomainException<Ship>(
                $"The ship cannot load at port {port} as it is docked at port {Port}");
        }

        AddEvent(new ShipEvents.Loading(Name, port));
    }

    public void FinishLoading(string port, double cargoWeight)
    {
        if (Status is not (ShipStatus.Loading or ShipStatus.UnUsed))
        {
            throw new DomainException<Ship>($"A ship cannot have finished loading when at status {Status}");
        }

        if (Port != port)
        {
            throw new DomainException<Ship>(
                $"The ship cannot finish loading at port {port} as it is docked at port {Port}");
        }

        AddEvent(new ShipEvents.Loaded(Name, port, cargoWeight));
    }

    public void Depart(string port)
    {
        if (Status is not (ShipStatus.AwaitingDeparture or ShipStatus.UnUsed))
        {
            throw new DomainException<Ship>($"A ship cannot depart when at status {Status}");
        }

        if (Port != port)
        {
            throw new DomainException<Ship>(
                $"The ship cannot depart {port} as it is awaiting departure at {Port}");
        }

        AddEvent(new ShipEvents.Departed(Name, port));
    }
}
```

### Disabling Sequencing

1. cover the reasons why you'd want to disable sequencing
    1. mention again the importance of the apply methods, even more so when using this model.
    1. explain the multi region write scenarios


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