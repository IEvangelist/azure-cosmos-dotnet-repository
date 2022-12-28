// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingCustomerAccount.Models;

public record CustomerAddress(
    string AddressLine1,
    string AddressLine2,
    string City,
    string Country,
    string PostCode);