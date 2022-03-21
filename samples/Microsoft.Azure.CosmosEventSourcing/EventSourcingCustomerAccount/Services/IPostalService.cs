// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using EventSourcingCustomerAccount.Models;

namespace EventSourcingCustomerAccount.Services;

public interface IPostalService
{
    ValueTask SendWelcomeLetterAsync(
        string firstName,
        string surname,
        CustomerAddress customerAddress);
}