using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Infrastructure;
using BasicEventSourcingSample.Projections;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosEventSourcing.Converters;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.Builders;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

services.AddSwaggerGen();
services.AddEndpointsApiExplorer();

services.AddCosmosEventStreaming();

services.AddCosmosRepository(options =>
{
    options.DatabaseId = "event-sourcing-sample";
    options.ContainerPerItemType = true;

    IItemContainerBuilder containerBuilder = options.ContainerBuilder;

    containerBuilder.Configure<ShipCosmosItem>(shipContainerOptions =>
    {
        shipContainerOptions.WithContainer("ships");
        shipContainerOptions.WithPartitionKey("/type");
    });

    containerBuilder.ConfigureEventSourcingContainer<SourcedShipEvent>("ship-tracking-events");
});

services.AddEventSourcingContainerProcessing<SourcedShipEvent, SourcedShipEventsProjectionBuilder>(options =>
{
    options.ProcessorName = "shipping-demo";
    options.InstanceName = Environment.MachineName;
});
services.AddCosmosRepositoryChangeFeedHostedService();

services.AddSingleton<IShipRepository, ShipRepository>();


WebApplication app = builder.Build();

app.Map("/", () => Results.Redirect("/swagger"));

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/ship", async (CreateShip createShip, IShipRepository shipRepository) =>
{
    (string name, DateTime dateTime) = createShip;
    Ship ship = new(name, dateTime);
    await shipRepository.CreateShip(ship);
});

app.MapPost("/api/ship/test", async (IEventSourcingRepository<SourcedShipEvent> repository) =>
{
    await repository.PersistAsync(new SourcedShipEvent(new ShipEvents.TestShipEvent(Guid.NewGuid().ToString()), "A"));
});

app.Run();

record CreateShip(string Name, DateTime Commissioned);