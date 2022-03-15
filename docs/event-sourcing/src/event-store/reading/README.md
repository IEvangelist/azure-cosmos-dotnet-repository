# Reading

The library offers some simple APIs in order to read your `EventItem`'s back. Some support reading all events, some support streaming events and some support querying the events in the store.

:::tip Tip
If you want to use an interface which only support's read operations the library offers the [`IReadOnlyEventStore<TEventItem>`](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/src/Microsoft.Azure.CosmosEventSourcing/Stores/IReadOnlyEventStore.cs)
:::

## Events

The library offers a set of methods on the `IEventStore<TEventItem>` that can be used to read the raw `EventItem`'s back from Cosmos. These can then be used to build up an `AggregateRoot`.

## Aggregate Roots

```csharp
//TODO:
```