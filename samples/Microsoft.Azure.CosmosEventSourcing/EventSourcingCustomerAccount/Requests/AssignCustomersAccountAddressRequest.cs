// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingCustomerAccount.Requests;

public record AssignCustomersAccountAddressRequest(
    string Username,
    string AddressLine1,
    string AddressLine2,
    string City,
    string Country,
    string PostCode);