using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Infrastructure;
using BasicEventSourcingSample.Projections;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.Builders;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddSwaggerGen();
services.AddEndpointsApiExplorer();
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

services.AddCosmosEventStreaming();
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

app.MapGet("/api/ships", async (IShipRepository shipRepository) =>
{
    IEnumerable<string> shipNames = await shipRepository.GetShipNamesAsync();
    return shipNames;
});

app.MapPost("/api/ships", async (CreateShip createShip, IShipRepository shipRepository) =>
{
    (string name, DateTime dateTime) = createShip;
    Ship ship = new(name, dateTime);
    await shipRepository.CreateShip(ship);
});

app.MapPost("/api/ships/dock", async (ShipEvents.DockedInPort docked, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(docked.Name);
    ship.Dock(docked.Port, docked.OccuredUtc);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/loading", async (ShipEvents.Loading loading, IShipRepository shipRepository) =>
{

});

app.MapPost("/api/ships/loaded", async (ShipEvents.Loaded loaded, IShipRepository shipRepository) =>
{

});

app.MapPost("/api/ships/departed", async (ShipEvents.Departed departed, IShipRepository shipRepository) =>
{

});

app.Run();

record CreateShip(string Name, DateTime Commissioned);