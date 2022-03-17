# Replaying Events

Now that we have stored an event in our Cosmos DB event store, how do we then change the `CustomerAccount` aggregate? In order to do this we need to read the events back from the event store and replay the events we currently have in the store to build our `AggregateRoot` back up to what is coined "current state". 

## Reading Events

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

The next step is to read the events back using the `IEventStore<TEventItem>`. This is shown on the below endpoint, it's also using the `Replay(...)` method we have defined on our `CustomerAccount`.

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
                x.DomainEvent).ToList());
    });
```

## Adding more Events

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

We can now use the `AssignAddress(...)` method then save our aggregate back into the event store. See the updated PUT endpoint below.

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
                x.DomainEvent).ToList());
        
        account.AssignAddress(
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.Country,
            request.PostCode);

        await eventStore.PersistAsync(
            account,
            account.Username);
    });
```

If we are to use this endpoint now we can see that we now have 3 events in the database, these are shown below.

You can make a curl request as defined below, or include swagger.

```bash
curl -X 'PUT' \
  'https://localhost:7273/api/accounts/address' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "username": "user123",
  "addressLine1": "100",
  "addressLine2": "Some Street",
  "city": "London",
  "country": "UK",
  "postCode": "LS1 7HG"
}'
```

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
            "username": "user123",
            "addressLine1": "100",
            "addressLine2": "Some Street",
            "city": "London",
            "country": "UK",
            "postCode": "LS1 7HG",
            "eventName": "CustomerAccountAddressAssigned",
            "sequence": 2,
            "occuredUtc": "2022-03-12T08:44:01.53703Z"
        },
        "partitionKey": "user123",
        "eventName": "CustomerAccountAddressAssigned",
        "_etag": "\"08002cb7-0000-0d00-0000-622c5d520000\"",
        "timeToLive": null,
        "createdTimeUtc": null,
        "id": "6d2ea1ac-950a-44df-bbcb-e23c9da72bbe",
        "type": "CustomerAccountEventItem",
        "_rid": "Gq1NAIsdmJwDAAAAAAAAAA==",
        "_self": "dbs/Gq1NAA==/colls/Gq1NAIsdmJw=/docs/Gq1NAIsdmJwDAAAAAAAAAA==/",
        "_attachments": "attachments/",
        "_ts": 1647074642
    },
    {
        "eventPayload": {
            "eventName": "AtomicEvent",
            "sequence": 2147483647,
            "occuredUtc": "2022-03-12T08:44:01.536736Z"
        },
        "partitionKey": "user123",
        "eventName": "AtomicEvent",
        "_etag": "\"08002db7-0000-0d00-0000-622c5d520000\"",
        "timeToLive": null,
        "createdTimeUtc": null,
        "id": "cb857440-d468-4887-bfbf-793d7968fc74",
        "type": "CustomerAccountEventItem",
        "_rid": "Gq1NAIsdmJwCAAAAAAAAAA==",
        "_self": "dbs/Gq1NAA==/colls/Gq1NAIsdmJw=/docs/Gq1NAIsdmJwCAAAAAAAAAA==/",
        "_attachments": "attachments/",
        "_ts": 1647074642
    }
]
```

:::tip Tip
Take your event sourcing implementation further with [Projections](../../projections/README.md)
:::
