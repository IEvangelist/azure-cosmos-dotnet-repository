# Introduction

This set of libraries helps implementing an event sourcing architecture with Cosmos DB. It aims to lower the entry barrier and the improve time to market of an application that would benefit from an ES architecture.

## Getting Setup

The easiest way to get started is with a simple .NET 6 web application to create one run the command below.

```bash
$ dotnet new web -n MyEventSourcingApplication
```

The next step is install the `IEvangelist.Azure.CosmosEventSourcing` nuget package. This can be done with the command below. This will give you a default web application.

```bash
cd MyEventSourcingApplication
dotnet add package IEvangelist.Azure.CosmosEventSourcing
```

## Domain Implementation

The first part of an event sourcing application is your domain. This follows a similar pattern as Domain Driven Design (DDD). This example will cover building a domain implementation of a customers account.

## Define an Aggregate Root

The next step is to define an `AggregateRoot`. This is an object that can change state and add new events, this can also replay events to build up current state. Let's define an `AggregateRoot` for a `CustomerAccount`.

```csharp
using EventSourcingCustomerAccount.Models;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;

namespace EventSourcingCustomerAccount.Aggregates;

public class CustomerAccount : AggregateRoot
{
    public string Username { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string Surname { get; private set; } = null!;

    public CustomerAddress? Address { get; private set; }

     public CustomerAccount(
        string username,
        string email,
        string firstName,
        string surname)
    {
        //TODO: Create Event
    }

    protected override void Apply(DomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }
}
```

This above example defines a class that represents a customer's account, note that this class uses the provided base class `AggregateRoot`. This class provides a few useful methods that we will need to use shortly.

## Define a Domain Event

The next step is to define a `DomainEvent` which represents a `CustomerAccount` being created. We will name this event `CustomerAccountCreated`. The implementation of this is shown below.

```csharp
using Microsoft.Azure.CosmosEventSourcing.Events;

namespace EventSourcingCustomerAccount.Events;

public record CustomerAccountCreated(
    string Username,
    string Email,
    string FirstName,
    string Surname) : DomainEvent;
``` 

This event in the above example is defined as a `record` this means by nature this event is immutable, this is a good fit as once an event has happened it should not be changed.

It also makes use of a provided type by the library. The `DomainEvent` offers a few things:

1. Gives this event a name, defaults to the name of the event, defaults to `this.GetType().Name`.
1. Defines a `Sequence` number which helps to keep track of the order events are applied.
1. Defines an `OccurredUtc` time to make when the event occurred.

:::tip Tip
`DomainEvent`'s are usually named in the past test as they represent something that _has_ happened.
:::

## Add a Domain Event

The next step is to add that event to the `AggregateRoot` there is a method provided on the utilized base type that let's us do this. It takes care of adding the `OccurredUtc` time stamp. It also sequentially increments the `Sequence` number on the event. Going back to the `CustomerAccount` example we can add this event in the constructor as below.

```csharp
namespace EventSourcingCustomerAccount.Aggregates;

public class CustomerAccount : AggregateRoot
{
    // Props excluded for brevity.

    public CustomerAccount(
        string username, 
        string email, 
        string firstName, 
        string surname, 
        CustomerAddress customerAddress)
    {
        AddEvent(new CustomerAccountCreated(
            username,
            email,
            firstName,
            surname));
    }

    // Apply method excluded for brevity.
}
```

:::tip Tip
In production applications this constructor should have guards, these would validate that the username was not empty for example.
:::

## Applying a Domain Event

Once a `DomainEvent` has been added the `AggregateRoot` will call the `Apply(...)` method that it enforces a consumer to implement. The easiest way to implement this is using a `switch` statement and pattern matching. This can then delegate the applying of that method off to another method for the specific `DomainEvent` that has been applied. See the `Apply(...)` methods used to apply the `CustomerAccountCreated` domain event.


```csharp
namespace EventSourcingCustomerAccount.Aggregates;

public class CustomerAccount : AggregateRoot
{
    // Props and Ctor excluded for brevity

    protected override void Apply(DomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CustomerAccountCreated created:
                Apply(created);
                break;
        }
    }

    private void Apply(CustomerAccountCreated created)
    {
        Username = created.Username;
        Email = created.Email;
        FirstName = created.FirstName;
        Surname = created.Surname;
    }
}
```

::: warning Notice
It is important that an `Apply(...)` method call _never_ fails. This is because this event has happened and is a _fact_. The behavior method that calls `AddEvent(...)` should be responsible for protecting any invalid events from being adding to the `AggregateRoot`.
:::

## Configuring a Store

In order to configure an event store to store all our events, we need to implement an `EventItem`. This is the type that will house our `DomainEvent`'s. It is used to configure an Azure Cosmos DB container in which we can save are events to. This `EventItem` will also allow us to define which property from our `AggregateRoot` should be used to partition our events.

Once we have defined an `EventItem` then we can configure the library. This stage of the guide will go through the steps required to achieve this.

## Defining an Event Item

In order to define an `EventItem` you need to define a class that implements `EventItem`. The library offers a type that implements this and supports storing `DomainEvent`'s. This class `DefaultEventItem` is used below.

```csharp
public class CustomerAccountEventItem : DefaultEventItem
{
    public CustomerAccountEventItem(
        string username,
        IDomainEvent domainEvent)
        : base(
            eventPayload: domainEvent,
            partitionKey: username)
    {
    }

    [JsonConstructor]
    public CustomerAccountEventItem(
        IDomainEvent eventPayload,
        string partitionKey) :
        base(eventPayload, partitionKey)
    {
    }
}
```

This class dictates that when this object is created it must be provided an `IDomainEvent` and a value to use to partition this `EventItem`.

::: warning Notice
The private constructor defined in the above example is noteworthy. This is required with the `[JsonConstructor]` attribute. The names of the parameters are also _very_ important to ensure de-serialization works correctly.
:::

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

The fist step is to give the database a name, this example then configures the event store using the container builder. This stores all events for a `CustomerAccount` in a container called `customer-accounts-events`.

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

## Replaying Events

Now that we have stored an event in our Cosmos DB event store, how do we then change the `CustomerAccount` aggregate? In order to do this we need to read the events back from the event store and replay the events we currently have in the store to build our `AggregateRoot` back up to what is coined "current state". 

In this example we are going to add a customer's address, this will first require us to read back our `CustomerAccount`. First we need to add a `static Replay(...)` method on our `CustomerAccount` this is show below.

```csharp
namespace EventSourcingCustomerAccount.Aggregates;

public class CustomerAccount : AggregateRoot
{
    // Excluded for brevity

    public static CustomerAccount Replay(List<DomainEvent> events)
    {
        CustomerAccount account = new();
        account.Apply(events);
        return account;
    }

    private CustomerAccount()
    {
        
    }
}
```

The above example defines a private constructor and a `static Replay(...)` method. This method takes a set of events and calls `Apply(...)` this overload will take the events, order them using the `Sequence` number and then apply's each event in order to our `AggregateRoot`. This builds it back up to current state, in this example it will re-apply our `CustomerAccountCreated` event.

The next step is to read the events back using the `IEventStore<TEventItem>`. This is shown on the below endpoint, it also using the `Replay(...)` method we have defined on our `CustomerAccount`.

```csharp
app.MapPut(
    "/api/accounts/address",
    async (AssignCustomersAccountAddressRequest request,
        IEventStore<CustomerAccountEventItem> eventStore) =>
    {
        IEnumerable<CustomerAccountEventItem> eventsItems =
            await eventStore.ReadAsync(request.Username);

        CustomerAccount account = CustomerAccount.Replay(
            eventsItems.Select(x =>
                x.DomainEventPayload).ToList());
    });
```

Now we have our customer account build back up, we can now apply another event which represents assigning the customers's account. This is added to the `CustomerAggregate` below.

```csharp
public class CustomerAccount : AggregateRoot
{
    //Excluded for brevity

    public void AssignAddress(
        string addressLine1,
        string addressLine2,
        string city,
        string country,
        string postCode)
    {
        // Guards excluded for brevity
        
        AddEvent(new CustomerAccountAddressAssigned(
            Username,
            addressLine1,
            addressLine2,
            city,
            country,
            postCode));
    }

    protected override void Apply(DomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CustomerAccountCreated created:
                Apply(created);
                break;
            case CustomerAccountAddressAssigned addressAssigned:
                Apply(addressAssigned);
                break;
        }
    }
    
    private void Apply(CustomerAccountAddressAssigned addressAssigned)
    {
        Address = new CustomerAddress(
            addressAssigned.AddressLine1,
            addressAssigned.AddressLine2,
            addressAssigned.City,
            addressAssigned.Country,
            addressAssigned.PostCode);
    }

    // Excluded for brevity
}
```

This follows the same pattern as when we created our `CustomerAccount`. This time we add a new event called `CustomerAccountAddressAssigned`. This has it's own `Apply(...)` method and it has also been added to the `switch` case.

We can now use the `AssignAddress(...)` method then save our aggregate back into the event store.