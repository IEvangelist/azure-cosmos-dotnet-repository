// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace EventSourcingCustomerAccount.Events;

public record CustomerAccountAddressAssigned(
    string Username,
    string AddressLine1,
    string AddressLine2,
    string City,
    string Country,
    string PostCode) : DomainEvent;