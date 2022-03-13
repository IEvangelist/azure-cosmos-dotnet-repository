# Domain Implementation

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