// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using EventSourcingCustomerAccount.Events;
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
        AddEvent(new CustomerAccountCreated(
            username,
            email,
            firstName,
            surname));
    }

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