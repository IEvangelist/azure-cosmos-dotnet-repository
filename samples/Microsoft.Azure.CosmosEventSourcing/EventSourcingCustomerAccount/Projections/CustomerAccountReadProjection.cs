// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingCustomerAccount.Events;
using EventSourcingCustomerAccount.Items;
using EventSourcingCustomerAccount.Models;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace EventSourcingCustomerAccount.Projections;

public record ReadProjectionKey : IProjectionKey;

public class CustomerAccountReadProjection :
    IEventItemProjection<CustomerAccountEventItem, ReadProjectionKey>
{
    private readonly IRepository<CustomerAccountReadItem> _repository;

    public CustomerAccountReadProjection(
        IRepository<CustomerAccountReadItem> repository) =>
        _repository = repository;

    public async ValueTask ProjectAsync(
        CustomerAccountEventItem eventItem,
        CancellationToken cancellationToken = default)
    {
        switch (eventItem.DomainEvent)
        {
            case CustomerAccountCreated created:
                await CreateProjection(created);
                break;
            case CustomerAccountAddressAssigned addressAssigned:
                await AssignAddressToProjection(addressAssigned);
                break;
        }
    }

    private async Task CreateProjection(
        CustomerAccountCreated created)
    {
        CustomerAccountReadItem readProjection = new(
            created.Username,
            created.Email,
            created.FirstName,
            created.Surname);

        await _repository.CreateAsync(readProjection);
    }

    private async Task AssignAddressToProjection(
        CustomerAccountAddressAssigned addressAssigned)
    {
        CustomerAccountReadItem readProjection =
            await _repository.GetAsync(addressAssigned.Username);

        readProjection.Address = new CustomerAddress(
            addressAssigned.AddressLine1,
            addressAssigned.AddressLine2,
            addressAssigned.City,
            addressAssigned.Country,
            addressAssigned.PostCode);

        await _repository.UpdateAsync(readProjection);
    }
}