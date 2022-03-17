# Configuring a Store

In order to configure an event store to store all our events, we need to implement an `EventItem`. This is the type that will house our `DomainEvent`'s. It is used to configure an Azure Cosmos DB container in which we can save are events to. This `EventItem` will also allow us to define which property from our `AggregateRoot` should be used to partition our events.

Once we have defined an `EventItem` then we can configure the library. This stage of the guide will go through the steps required to achieve this.

## Defining an Event Item

In order to define an `EventItem` you need to define a class that implements `EventItem`. The library offers a type that implements this and supports storing `DomainEvent`'s. This class `EventItem` is used below.

```csharp
public class CustomerAccountEventItem : EventItem
{
    public CustomerAccountEventItem(
        string username,
        DomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
        PartitionKey = username;
    }
}
```

This class dictates that when this object is created it must be provided a `DomainEvent` and a value to use to partition this `EventItem`.

## Startup Configuration

We now have all we need to configure the library and start storing our events. The library follows the common pattern of implementing an extension method for the `IServiceCollection` this make it nice and easy to get started configuring this library.

The first thing to do is call `.AddCosmosEventSourcing(...)` this then allows us to configure different parts of the event sourcing implementation.


```csharp
using EventSourcingCustomerAccount.Items;
using Microsoft.Azure.CosmosEventSourcing.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(options =>
    {
        options.DatabaseId = "customer-accounts-sample-db";
        options.ContainerBuilder
            .ConfigureEventItemStore<CustomerAccountEventItem>(
                "customer-account-events");
    });

    eventSourcingBuilder.AddDomainEventTypes(typeof(Program).Assembly);
});

// Excluded for brevity
```

The first step is to give the database a name, this example then configures the event store using the container builder. This stores all events for a `CustomerAccount` in a container called `customer-accounts-events`.

The last part of this example makes a call to `.AddDomainEventTypes(...)`, this scans the assemblies provided for all types that implement `IDomainEvent`. This is required to perform the polymorphic de-serialization of different event payload on each `EventItem`.

## Persisting Events

Now we have an `AggregateRoot`, some `DomainEvent`'s and we have configured the library, we can now persist some events. In order to this we can make use of the `IEventStore<TEventItem>` interface.

In this web application example we will define a HTTP Post request that will take some information that can be used to create our customer's account. This will then create an `CustomerAccount` and persist this `AggregateRoot` using the `IEventStore<TEventItem>`.

```csharp
// Excluded for brevity

WebApplication app = builder.Build();

app.MapGet("/", () => "Event Sourcing - Customer Accounts");

app.MapPost(
    "/api/accounts/", async (CreateCustomerAccountRequest request,
        IEventStore<CustomerAccountEventItem> eventStore) =>
    {
        CustomerAccount account = new(
            request.Username,
            request.Email,
            request.FirstName,
            request.Surname);

        await eventStore.PersistAsync(
            aggregateRoot: account, 
            partitionKeyValue: account.Username);
    });

app.Run();
```

Before running the application there is one final thing that needs to be done. We need to provide the library with a connection string to an Azure Cosmos DB account. This can be set using `dotnet user-secrets`. See below.

```bash
$ dotnet user-secrets init
$ dotnet user-secrets set RepositoryOptions:CosmosConnectionString "<your-conn-string-here>"
```

Now we are ready to run the application. You can run `dotnet run` from the command line to start the app, then make a POST request to `/api/accounts/` to persist our first event. See the `curl` request below.

```bash
$ curl -X POST -H "Content-Type: application/json" \
    -d '{"username": "user123", "email": "linuxize@example.com", "firstName" : "Joe", "surname": "Bloggs"}' \
    http://localhost:5238/api/accounts/
```

If you now check the `customer-account-events` container you will find 2 events, these are shown below.

```json
[
    {
        "eventPayload": {
            "username": "user123",
            "email": "linuxize@example.com",
            "firstName": "Joe",
            "surname": "Bloggs",
            "eventName": "CustomerAccountCreated",
            "sequence": 1,
            "occuredUtc": "2022-03-11T22:47:49.268682Z"
        },
        "partitionKey": "user123",
        "eventName": "CustomerAccountCreated",
        "_etag": "\"0000220d-0000-0d00-0000-622bd1a20000\"",
        "timeToLive": null,
        "createdTimeUtc": null,
        "id": "556cd0f1-0372-429e-b2d8-f1adfc5cfddd",
        "type": "CustomerAccountEventItem",
        "_rid": "Gq1NAIsdmJwBAAAAAAAAAA==",
        "_self": "dbs/Gq1NAA==/colls/Gq1NAIsdmJw=/docs/Gq1NAIsdmJwBAAAAAAAAAA==/",
        "_attachments": "attachments/",
        "_ts": 1647038882
    },
    {
        "eventPayload": {
            "eventName": "AtomicEvent",
            "sequence": 2147483647,
            "occuredUtc": "2022-03-11T22:47:49.268433Z"
        },
        "partitionKey": "user123",
        "eventName": "AtomicEvent",
        "_etag": "\"0000230d-0000-0d00-0000-622bd1a20000\"",
        "timeToLive": null,
        "createdTimeUtc": null,
        "id": "cb857440-d468-4887-bfbf-793d7968fc74",
        "type": "CustomerAccountEventItem",
        "_rid": "Gq1NAIsdmJwCAAAAAAAAAA==",
        "_self": "dbs/Gq1NAA==/colls/Gq1NAIsdmJw=/docs/Gq1NAIsdmJwCAAAAAAAAAA==/",
        "_attachments": "attachments/",
        "_ts": 1647038882
    }
]
```

:::tip Tip
The `AtomicEvent` stored is covered in more detail [here](). This is used to guarantee ACID like qualities when saving events.
:::