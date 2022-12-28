// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingCustomerAccount.Requests;

public record CreateCustomerAccountRequest(
    string Username,
    string Email,
    string FirstName,
    string Surname);