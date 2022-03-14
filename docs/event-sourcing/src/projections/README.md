# Projections

Projections allow you to build up an alternate view of your events, notify system's outside your domains boundary of this event and more! This is done by processing all new events in a separate process much like you would process events off of a queue.

This is fully supported in the library by making using of a feature of Cosmos DB called the change feed. The change feed is a persistent log of writes and updates to any items in a container. This means whenever we write a new event, this will be picked up by the change feed. We can then setup a background worker to process these changes.

Projections can be used as a read store to show current state of an `AggregateRoot` _without_ having to replay all the events. For `AggregateRoot`'s that have a lot of events this can often be a good fit. You could also take the opportunity to take this internal event to your application and offer that to other systems by taking this event and placing it on a message broker for example.

```csharp
//TODO: MORE DOCS COMING!
```