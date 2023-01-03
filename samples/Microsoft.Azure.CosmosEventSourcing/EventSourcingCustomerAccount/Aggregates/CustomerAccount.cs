// Copyright (c) David Pine. All rights reserved.
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

    public void AssignAddress(
        string addressLine1,
        string addressLine2,
        string city,
        string country,
        string postCode) =>
        // Guards excluded for brevity

        AddEvent(new CustomerAccountAddressAssigned(
            Username,
            addressLine1,
            addressLine2,
            city,
            country,
            postCode));

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

    private void Apply(CustomerAccountAddressAssigned addressAssigned) => Address = new CustomerAddress(
            addressAssigned.AddressLine1,
            addressAssigned.AddressLine2,
            addressAssigned.City,
            addressAssigned.Country,
            addressAssigned.PostCode);

    private void Apply(CustomerAccountCreated created)
    {
        Username = created.Username;
        Email = created.Email;
        FirstName = created.FirstName;
        Surname = created.Surname;
    }

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