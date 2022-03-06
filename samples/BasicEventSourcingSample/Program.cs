using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Infrastructure;
using BasicEventSourcingSample.Projections.Models;
using CleanArchitecture.Exceptions.AspNetCore;
using Microsoft.Azure.CosmosEventSourcing;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddCleanArchitectureExceptionsHandler(options => options.ApplicationName = "EventSourcingShipSample");
services.AddSwaggerGen();
services.AddEndpointsApiExplorer();

services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(options =>
    {
        options.DatabaseId = "event-sourcing-shipping-sample";
        options.ContainerBuilder
            .ConfigureProjectionStore<ShipInformation>("ship-projections")
            .ConfigureEventSourceStore<ShipEventSource>("ship-tracking-events");
    });

    eventSourcingBuilder.AddAllPersistedEventsTypes();
    eventSourcingBuilder.AddAllEventProjectionHandlers();
    eventSourcingBuilder.AddEventBasedEventSourceProjectionBuilder<ShipEventSource>(options =>
    {
        options.ProcessorName = "shipping-demo";
        options.InstanceName = Environment.MachineName;
    });
});

services.AddCosmosRepositoryChangeFeedHostedService();

services.AddSingleton<IShipRepository, ShipRepository>();

WebApplication app = builder.Build();

app.Map("/", () => Results.Redirect("/swagger"));

app.UseCleanArchitectureExceptionsHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/ships", async (IShipRepository shipRepository) =>
{
    IEnumerable<string> shipNames = await shipRepository.GetShipNamesAsync();
    return shipNames;
});

app.MapGet("/api/ships/info/{name}", async (string name, IShipRepository shipRepository) =>
{
    ShipInformation? information = await shipRepository.GetInformationAsync(name);

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

app.MapGet("/api/ship/{shipName}/departures", async (string shipName, IEventStore<ShipEventSource> store) =>
{
    IEnumerable<ShipEventSource> events = await store.ReadAsync(
        shipName,
        x => x.EventName == nameof(ShipEvents.Departed));

    List<ShipEvents.Departed> departedEvents = events
        .Select(x =>
            x.GetEventPayload<ShipEvents.Departed>())
        .OrderBy(x => x.OccuredUtc)
        .ToList();

    return departedEvents.Select(x =>
        new ShipDepartureDto(x.Name, x.Port, x.OccuredUtc));
});

app.Run();

record CreateShip(string Name, DateTime Commissioned);

record ShipDepartureDto(string Name, string Port, DateTime OccuredUtc);

record ShipInfoDto(string Name, DateTime Commissioned, string? LatestPort, double? LatestCargoWeight);