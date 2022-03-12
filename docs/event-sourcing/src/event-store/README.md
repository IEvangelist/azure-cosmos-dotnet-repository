# Event Store

The library provides a simple `interface` that allows you to interact with Azure Cosmos DB as an event store.

This interface is the `IEventStore<TEventItem>`, this allows you to save events, save aggregate roots that holds events. Read all events, stream events using `IAsyncEnumerable<T>` and query events to limit the set of events that are returned.