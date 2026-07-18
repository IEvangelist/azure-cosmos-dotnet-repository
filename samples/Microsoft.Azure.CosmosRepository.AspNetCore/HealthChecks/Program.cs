// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using HealthChecks.Items;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add Cosmos Repository with health checks
builder.Services.AddCosmosRepository(options =>
{
    options.DatabaseId = "health-check-demo";
    options.ContainerPerItemType = true;
    options.ContainerBuilder.Configure<Product>(containerOptions =>
    {
        containerOptions.WithContainer("products");
        containerOptions.WithPartitionKey("/category");
    });
});

// Add health checks for Cosmos Repository
builder.Services
    .AddHealthChecks()
    .AddCosmosRepository();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Eagerly initialize Cosmos containers before starting the application
// This ensures containers exist before health checks run
// Product type is discovered from ContainerBuilder.Configure<Product>() above
await app.Services.EagerlyInitializeCosmosContainersAsync();

app.UseSwagger();
app.UseSwaggerUI();

// Map health check endpoints
app.MapHealthChecks("/health");

// Simple API endpoints
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/api/products", async (IRepository<Product> repository) =>
{
    var products = await repository.GetAsync(p => true);
    return Results.Ok(products);
})
.WithName("GetProducts");

app.MapPost("/api/products", async (Product product, IRepository<Product> repository) =>
{
    var created = await repository.CreateAsync(product);
    return Results.Created($"/api/products/{created.Id}", created);
})
.WithName("CreateProduct");

app.Run();
