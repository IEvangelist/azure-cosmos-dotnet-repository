// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingCustomerAccount.Events;
using EventSourcingCustomerAccount.Items;
using EventSourcingCustomerAccount.Models;
using EventSourcingCustomerAccount.Services;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingCustomerAccount.Projections;

public record WelcomeLetterProjectionKey : IProjectionKey;

public class WelcomeLetterProjection :
    IEventItemProjection<CustomerAccountEventItem, WelcomeLetterProjectionKey>
{
    private readonly IReadOnlyRepository<CustomerAccountReadItem> _repository;
    private readonly IPostalService _postalService;

    public WelcomeLetterProjection(
        IReadOnlyRepository<CustomerAccountReadItem> repository,
        IPostalService postalService)
    {
        _repository = repository;
        _postalService = postalService;
    }

    public async ValueTask ProjectAsync(
        CustomerAccountEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        if (eventItem.DomainEvent is CustomerAccountAddressAssigned addressAssigned)
        {
            CustomerAccountReadItem? account = await _repository.TryGetAsync(
                addressAssigned.Username,
                cancellationToken: cancellationToken);

            if (account is not null)
            {
                await _postalService.SendWelcomeLetterAsync(
                    account.FirstName,
                    account.Surname,
                    new CustomerAddress(
                        addressAssigned.AddressLine1,
                        addressAssigned.AddressLine2,
                        addressAssigned.City,
                        addressAssigned.Country,
                        addressAssigned.PostCode));
            }
        }
    }
}