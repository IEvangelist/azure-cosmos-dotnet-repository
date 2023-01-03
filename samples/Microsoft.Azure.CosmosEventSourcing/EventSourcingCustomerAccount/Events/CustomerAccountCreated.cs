// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace EventSourcingCustomerAccount.Events;

public record CustomerAccountCreated(
    string Username,
    string Email,
    string FirstName,
    string Surname) : DomainEvent;