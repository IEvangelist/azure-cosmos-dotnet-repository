// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

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