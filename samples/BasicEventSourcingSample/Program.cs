using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Infrastructure;
using BasicEventSourcingSample.Projections.Models;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.Builders;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddSwaggerGen();
services.AddEndpointsApiExplorer();
services.AddCosmosRepository(options =>
{
    options.DatabaseId = "event-sourcing-shipping-sample";
    options.ContainerPerItemType = true;

    IItemContainerBuilder containerBuilder = options.ContainerBuilder;

    containerBuilder.Configure<ShipInformation>(shipContainerOptions =>
    {
        shipContainerOptions.WithContainer("ships-projections");
        shipContainerOptions.WithPartitionKey("/partitionKey");
    });

    containerBuilder.ConfigureEventSourceStore<ShipEventSource>("ship-tracking-events");
});

services.AddCosmosEventSourcing();
services.AddEventSourceProjectionBuilder<ShipEventSource>(options =>
{
    options.ProcessorName = "shipping-demo";
    options.InstanceName = Environment.MachineName;
});
services.AddAllEventProjectionHandlers();
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

app.MapGet("/api/ships/info/{name}", async (string name, IShipRepository shipRepository) =>
{
    ShipInformation? information = await shipRepository.GetInformation(name);

    if (information is null)
    {
        return Results.NoContent();
    }

    ShipInfoDto dto = new(
        information.Name,
        information.Commissioned,
        information.LatestPort,
        information.LatestCargoWeight);

    return Results.Ok(dto);
});

app.MapPost("/api/ships", async (CreateShip createShip, IShipRepository shipRepository) =>
{
    (string name, DateTime dateTime) = createShip;
    Ship ship = new(name, dateTime);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/dock", async (ShipEvents.DockedInPort docked, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(docked.Name);
    ship.Dock(docked.Port, DateTime.UtcNow);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/loading", async (ShipEvents.Loading loading, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(loading.Name);
    ship.StartLoading(loading.Port, DateTime.UtcNow);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/loaded", async (ShipEvents.Loaded loaded, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(loaded.Name);
    ship.FinishLoading(loaded.Port, loaded.CargoWeight, DateTime.UtcNow);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/departed", async (ShipEvents.Departed departed, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(departed.Name);
    ship.Depart(departed.Port, DateTime.UtcNow);
    await shipRepository.SaveAsync(ship);
});

app.Run();

record CreateShip(string Name, DateTime Commissioned);

record ShipInfoDto(string Name, DateTime Commissioned, string? LatestPort, double? latestCargoWeight);