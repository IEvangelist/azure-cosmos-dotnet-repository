// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using EventSourcingCustomerAccount.Aggregates;
using EventSourcingCustomerAccount.Items;
using EventSourcingCustomerAccount.Projections;
using EventSourcingCustomerAccount.Requests;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwaggerGen()
    .AddEndpointsApiExplorer();

builder.Services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(options =>
    {
        options.DatabaseId = "customer-accounts-sample-db";
        options.ContainerBuilder
            .ConfigureEventItemStore<CustomerAccountEventItem>(
                "customer-account-events")
            .ConfigureProjectionStore<CustomerAccountReadItem>(
                containerName: "projections",
                partitionKey: "/username");
    });

    eventSourcingBuilder
        .AddEventItemProjection<CustomerAccountEventItem,
            ReadProjectionKey,
            CustomerAccountReadProjection>(
            options =>
            {
                options.ProcessorName =
                    "customer-account-read-projection-builder";

                options.InstanceName =
                    Environment.MachineName;
            });

    eventSourcingBuilder
        .AddEventItemProjection<CustomerAccountEventItem,
            WelcomeLetterProjectionKey,
            WelcomeLetterProjection>(
            options =>
            {
                options.ProcessorName = "welcome-letter-builder";
                options.InstanceName = Environment.MachineName;
            });

    eventSourcingBuilder.AddDomainEventTypes(typeof(Program).Assembly);
});

builder.Services.AddCosmosRepositoryChangeFeedHostedService();

WebApplication app = builder.Build();

app.MapGet("/", () => "Event Sourcing - Customer Accounts");

app
    .UseSwagger()
    .UseSwaggerUI();

app.MapPost(
    "/api/accounts/", async (CreateCustomerAccountRequest request,
        IEventStore<CustomerAccountEventItem> eventStore) =>
    {
        CustomerAccount account = new(
            request.Username,
            request.Email,
            request.FirstName,
            request.Surname);

        await eventStore.PersistAsync(
            aggregateRoot: account,
            partitionKeyValue: account.Username);
    });

app.MapPut(
    "/api/accounts/address",
    async (AssignCustomersAccountAddressRequest request,
        IEventStore<CustomerAccountEventItem> eventStore) =>
    {
        IEnumerable<CustomerAccountEventItem> eventsItems =
            await eventStore.ReadAsync(request.Username);

        var account = CustomerAccount.Replay(
            eventsItems.Select(x =>
                x.DomainEvent).ToList());

        account.AssignAddress(
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.Country,
            request.PostCode);

        await eventStore.PersistAsync(
            account,
            account.Username);
    });

app.Run();