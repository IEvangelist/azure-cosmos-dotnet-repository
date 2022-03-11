using EventSourcingCustomerAccount.Aggregates;
using EventSourcingCustomerAccount.Items;
using EventSourcingCustomerAccount.Models;
using EventSourcingCustomerAccount.Requests;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Stores;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(options =>
    {
        options.DatabaseId = "customer-accounts-sample-db";
        options.ContainerBuilder
            .ConfigureEventItemStore<CustomerAccountEventItem>(
                "customer-account-events");
    });

    eventSourcingBuilder.AddDomainEventTypes(typeof(Program).Assembly);
});

WebApplication app = builder.Build();

app.MapGet("/", () => "Event Sourcing - Customer Accounts");

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

app.Run();