// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingCustomerAccount.Models;

public record SentWelcomeLetter(string FirstName, string Surname, CustomerAddress Address);