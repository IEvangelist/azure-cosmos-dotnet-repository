# Building a Read Projection

The library also supports building projections which are done out of process and make use of Azure Cosmos DBs change feed. For more information on how the change feed works and how this library utilizes it see the [projections documentation here.](../../projections/README.md)

A read projection in this example is going to store a single document that represents current state. The reason for this is that let's say we expand on our `CustomerAccount` example and we add more and more events. This is great and totally the idea of event sourcing, however, if we needed to serve the current state of a customer's account quickly via an API call for example. Then replaying these events to build up current state might not be the most efficient way to do this. 

This is where a read projection comes into play, every time a new event is added we can process this and apply it to our read only projection. This can then be read with a single read to cosmos and we immediately have current state.

:::tip Tip
There is a fantastic video on projections by Derek Comartin this can be found on his [YouTube channel CodeOpinion here.](https://www.youtube.com/watch?v=bTRjO6JK4Ws)
:::

## Defining a Read Model

The library makes use of the [`IEvangelist.Azure.CosmosRepository`](https://ievangelist.github.io/azure-cosmos-dotnet-repository/) to store it's event, this means you can also use this package to store your read projections or any other data that you want!

The first step is to define a class that will represent our `CustomerAccount` read model. This is defined below.

```csharp
using EventSourcingCustomerAccount.Models;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingCustomerAccount.Items;

public class CustomerAccountReadItem : FullItem
{
    public string Username { get; }

    public string Email { get; }

    public string FirstName { get; }

    public string Surname { get; }

    public CustomerAddress? Address { get; set; }

    protected override string GetPartitionKeyValue() =>
        Username;

    public CustomerAccountReadItem(
        string username, 
        string email, 
        string firstName, 
        string surname)
    {
        Id = username;
        Username = username;
        Email = email;
        FirstName = firstName;
        Surname = surname;
    }
}
```

This is a simple POCO class that will be serialized to and from JSON by the library and stored in a container. [This make use of a `FullItem` base `class` read more on this here.](https://github.com/IEvangelist/azure-cosmos-dotnet-repository#optimistic-concurrency-control-with-etags)

:::tip Tip
You do not have to use Azure Cosmos DB to store your models you build via projections you can use whatever you like!
:::

## Configuring Storage

The next step is to configure somewhere for this projection to be stored. To do this we can use an extension method provided by the library. We simply need to extend our `.AddCosmosEventSourcing(...)` implementation.

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

    eventSourcingBuilder.AddDomainEventTypes(typeof(Program).Assembly);
});
```

In the above example we are saying use a new container called `projections` and we are saying these projections will be partitioned by `/username`.

## Building the Read Model

In order to build a projection the first step is to implement the interface`IEventItemProjection<TEventItem, TProjectionKey>`. Once you have this you get the opportunity to process the events that you want. This follows a similar pattern to the `Apply(...)` methods used in the domain implementation. However, here we can only process the events that we care about for our current projection. See the example projection below.

> The TProjectionKey is used to allow multiple projections on the same host.

```csharp
// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

public record ReadProjectionKey : IProjectionKey;

using EventSourcingCustomerAccount.Events;
using EventSourcingCustomerAccount.Items;
using EventSourcingCustomerAccount.Models;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingCustomerAccount.Projections;

public class CustomerAccountReadProjection :
    IEventItemProjection<CustomerAccountEventItem, ReadProjectionKey>
{
    private readonly IRepository<CustomerAccountReadItem> _repository;

    public CustomerAccountReadProjection(
        IRepository<CustomerAccountReadItem> repository) =>
        _repository = repository;

    public async ValueTask ProjectAsync(
        CustomerAccountEventItem sourcedEvent,
        CancellationToken cancellationToken = default)
    {
        switch (sourcedEvent.DomainEvent)
        {
            case CustomerAccountCreated created:
                await CreateProjection(created);
                break;
            case CustomerAccountAddressAssigned addressAssigned:
                await AssignAddressToProjection(addressAssigned);
                break;
        }
    }

    private async Task CreateProjection(
        CustomerAccountCreated created)
    {
        CustomerAccountReadItem readProjection = new(
            created.Username,
            created.Email,
            created.FirstName,
            created.Surname);

        await _repository.CreateAsync(readProjection);
    }

    private async Task AssignAddressToProjection(
        CustomerAccountAddressAssigned addressAssigned)
    {
        CustomerAccountReadItem readProjection =
            await _repository.GetAsync(addressAssigned.Username);

        readProjection.Address = new CustomerAddress(
            addressAssigned.AddressLine1,
            addressAssigned.AddressLine2,
            addressAssigned.City,
            addressAssigned.Country,
            addressAssigned.PostCode);

        await _repository.UpdateAsync(readProjection);
    }
}
```

## Configuring Projection

Now we have our `IEventItemProjection<CustomerAccountEventItem, ReadProjectionKey>` we need to tell the library to use it. This is done via again extending the `.AddCosmosEventSourcing(...)` implementation. See an example of this below.

```csharp
builder.Services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    // Excluded for brevity

    eventSourcingBuilder
        .AddEventItemProjection<CustomerAccountEventItem,
            ReadProjectionKey,
            CustomerAccountReadProjection>(
            options =>
            {
                options.ProcessorName =
                    "customer-account-read-projection-builder";

                options.InstanceName =
                    Environment.MachineName;
            });

    eventSourcingBuilder.AddDomainEventTypes(typeof(Program).Assembly);
});
```

The above example uses the `AddEventItemProjection<TEventItem, TProjectionKey, TProjection>(...)` method. Your first specify the `EventItem` type you are going to process and second the builder that will process the events.

You then specify a processor name, this is the description of the processors intent. The next is the instance name, this is the physical processor. This is used when load balancing changes across multiple nodes. You can read more on how the [Azure Cosmos DBs change feed processor library uses this here.](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/change-feed-processor)

## Configure a Background Service

Once you have this configured we need to make use another library which provides an implementation of a `BackgroundService` which will listen to changes from the change feed on a background process.

First install the [`IEvangelist.Azure.CosmosRepository.AspNetCore`](https://www.nuget.org/packages/IEvangelist.Azure.CosmosRepository.AspNetCore/) package to the project.

```bash
$ cd MyEventSourcingApplication
$ dotnet add package IEvangelist.Azure.CosmosRepository.AspNetCore
```

Then we can use the extension method provided to run the background process to listen to changes.

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Excluded for brevity

builder.Services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    // Excluded for brevity
});

builder.Services.AddCosmosRepositoryChangeFeedHostedService();

WebApplication app = builder.Build();
```

## Try it out!

The next step is to run the application, create an account and then assign it an address via the API endpoints setup earlier. Then check the `projections` container for the newly built projection!

See an example projection below

```json
{
    "username": "user321",
    "email": "user321@domain.com",
    "firstName": "Fred",
    "surname": "Jones",
    "address": {
        "addressLine1": "100 Street 10",
        "addressLine2": "Town A",
        "city": "London",
        "country": "UK",
        "postCode": "LS1 7YH"
    },
    "_etag": "\"04004c95-0000-0d00-0000-622dd78a0000\"",
    "timeToLive": null,
    "createdTimeUtc": "2022-03-13T11:35:36.737351Z",
    "id": "user321",
    "type": "CustomerAccountReadItem",
    "_rid": "Gq1NAPDGLZQBAAAAAAAAAA==",
    "_self": "dbs/Gq1NAA==/colls/Gq1NAPDGLZQ=/docs/Gq1NAPDGLZQBAAAAAAAAAA==/",
    "_attachments": "attachments/",
    "_ts": 1647171466
}
```

The corresponding events that created this projection are also shown below. The query used to pull these back in the azure portal is also shown.

```sql
-- Run against the customer-account-events container

SELECT * FROM c
where c.partitionKey = 'user321'
order by c.eventPayload["sequence"]
```

```json
[
    {
        "eventPayload": {
            "username": "user321",
            "email": "user321@domain.com",
            "firstName": "Fred",
            "surname": "Jones",
            "eventName": "CustomerAccountCreated",
            "sequence": 1,
            "occuredUtc": "2022-03-13T11:35:28.437917Z"
        },
        "partitionKey": "user321",
        "eventName": "CustomerAccountCreated",
        "_etag": "\"0b002785-0000-0d00-0000-622dd7010000\"",
        "timeToLive": null,
        "createdTimeUtc": null,
        "id": "7205f507-877e-498e-a405-ad0324ec1dc1",
        "type": "CustomerAccountEventItem",
        "_rid": "Gq1NAIsdmJwEAAAAAAAAAA==",
        "_self": "dbs/Gq1NAA==/colls/Gq1NAIsdmJw=/docs/Gq1NAIsdmJwEAAAAAAAAAA==/",
        "_attachments": "attachments/",
        "_ts": 1647171329
    },
    {
        "eventPayload": {
            "username": "user321",
            "addressLine1": "100 Street 10",
            "addressLine2": "Town A",
            "city": "London",
            "country": "UK",
            "postCode": "LS1 7YH",
            "eventName": "CustomerAccountAddressAssigned",
            "sequence": 2,
            "occuredUtc": "2022-03-13T11:37:42.093858Z"
        },
        "partitionKey": "user321",
        "eventName": "CustomerAccountAddressAssigned",
        "_etag": "\"0b002f86-0000-0d00-0000-622dd7860000\"",
        "timeToLive": null,
        "createdTimeUtc": null,
        "id": "84864304-ad96-4a0a-9832-1c8a56e9ad46",
        "type": "CustomerAccountEventItem",
        "_rid": "Gq1NAIsdmJwGAAAAAAAAAA==",
        "_self": "dbs/Gq1NAA==/colls/Gq1NAIsdmJw=/docs/Gq1NAIsdmJwGAAAAAAAAAA==/",
        "_attachments": "attachments/",
        "_ts": 1647171462
    },
    {
        "eventPayload": {
            "eventName": "AtomicEvent",
            "sequence": 2147483647,
            "occuredUtc": "2022-03-13T11:37:42.093597Z"
        },
        "partitionKey": "user321",
        "eventName": "AtomicEvent",
        "_etag": "\"0b003086-0000-0d00-0000-622dd7860000\"",
        "timeToLive": null,
        "createdTimeUtc": null,
        "id": "5189e4f0-d4c2-47e3-9b53-3a73c8c25587",
        "type": "CustomerAccountEventItem",
        "_rid": "Gq1NAIsdmJwFAAAAAAAAAA==",
        "_self": "dbs/Gq1NAA==/colls/Gq1NAIsdmJw=/docs/Gq1NAIsdmJwFAAAAAAAAAA==/",
        "_attachments": "attachments/",
        "_ts": 1647171462
    }
]
```