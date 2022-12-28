// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingCustomerAccount.Models;

namespace EventSourcingCustomerAccount.Services;

public class DefaultPostalService : IPostalService
{
    private readonly ILogger<DefaultPostalService> _logger;

    public DefaultPostalService(ILogger<DefaultPostalService> logger) =>
        _logger = logger;

    public ValueTask SendWelcomeLetterAsync(
        string firstName,
        string surname,
        CustomerAddress customerAddress)
    {
        _logger.LogInformation("Sending welcome letter to {Name} with address {AddressDetails}",
            $"{firstName} {surname}",
            customerAddress);

        return ValueTask.CompletedTask;
    }
}