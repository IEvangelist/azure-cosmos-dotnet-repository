// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices.Context;
using BasicEventSourcingSample.Core;
using BasicEventSourcingSample.Infrastructure;
using BasicEventSourcingSample.Projections;
using BasicEventSourcingSample.Projections.Models;
using CleanArchitecture.Exceptions.AspNetCore;
using CorrelationId;
using CorrelationId.Abstractions;
using CorrelationId.DependencyInjection;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;


services.AddHealthChecks().AddCosmosRepository();
services.AddCleanArchitectureExceptionsHandler(options => options.ApplicationName = "EventSourcingShipSample");
services.AddSwaggerGen();
services.AddEndpointsApiExplorer();
services.AddDefaultCorrelationId();

services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(options =>
    {
        options.DatabaseId = "event-sourcing-shipping-sample";
        options.ContainerBuilder
            .ConfigureProjectionStore<ShipInformation>("ship-projections")
            .ConfigureEventItemStore<ShipEventItem>("ship-tracking-events");
    });

    eventSourcingBuilder.AddDomainEventTypes();
    eventSourcingBuilder.AddDomainEventProjectionHandlers();
    eventSourcingBuilder.AddDefaultDomainEventProjection<ShipEventItem, ShipInformationProjectionKey>(options =>
    {
        options.ProcessorName = "shipping-demo";
        options.InstanceName = Environment.MachineName;
    });
});

services.AddCosmosRepositoryChangeFeedHostedService();

services.AddScoped<IShipRepository, ShipRepository>();

WebApplication app = builder.Build();

app.Map("/", () => Results.Redirect("/swagger"));

app.UseCleanArchitectureExceptionsHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCorrelationId();

app.Use(async (context, next) =>
{
    ICorrelationContextAccessor correlationContextAccessor =
        context.RequestServices.GetRequiredService<ICorrelationContextAccessor>();
    IContextService contextService = context.RequestServices.GetRequiredService<IContextService>();
    contextService.CorrelationId = correlationContextAccessor.CorrelationContext.CorrelationId;
    await next.Invoke();
});

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
    (var name, DateTime dateTime) = createShip;
    Ship ship = new(name, dateTime);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/dock", async (ShipEvents.DockedInPort docked, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(docked.Name);
    ship.Dock(docked.Port);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/loading", async (ShipEvents.Loading loading, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(loading.Name);
    ship.StartLoading(loading.Port);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/loaded", async (ShipEvents.Loaded loaded, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(loaded.Name);
    ship.FinishLoading(loaded.Port, loaded.CargoWeight);
    await shipRepository.SaveAsync(ship);
});

app.MapPost("/api/ships/departed", async (ShipEvents.Departed departed, IShipRepository shipRepository) =>
{
    Ship ship = await shipRepository.FindAsync(departed.Name);
    ship.Depart(departed.Port);
    await shipRepository.SaveAsync(ship);
});

app.MapGet("/api/ship/{shipName}/departures", async (string shipName, IEventStore<ShipEventItem> store) =>
{
    IEnumerable<ShipEventItem> events = await store.ReadAsync(
        shipName,
        x => x.EventName == nameof(ShipEvents.Departed));

    var departedEvents = events
        .Select(x =>
            x.GetEventPayload<ShipEvents.Departed>())
        .OrderBy(x => x.OccuredUtc)
        .ToList();

    return departedEvents.Select(x =>
        new ShipDepartureDto(x.Name, x.Port, x.OccuredUtc));
});

app.Run();

internal record CreateShip(string Name, DateTime Commissioned);

internal record ShipDepartureDto(string Name, string Port, DateTime OccuredUtc);

internal record ShipInfoDto(string Name, DateTime Commissioned, string? LatestPort, double? LatestCargoWeight);