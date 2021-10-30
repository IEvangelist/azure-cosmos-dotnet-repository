using Microsoft.AspNetCore.Mvc;
using Services.Products.Application.Commands;
using Services.Products.Application.Services;
using Services.Products.Domain.Entities;
using Services.Products.Infrastructure.Cosmos;
using Services.Shared;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository(options =>
{
    options.ContainerPerItemType = true;
    options.ContainerBuilder
        .Configure<Product>(itemBuilder =>
        {
            itemBuilder
                .WithContainer(CosmosConstants.Containers.ProductsContainer)
                .WithPartitionKey(CosmosConstants.PartitionKeys.ProductsPartitionKey)
                .WithManualThroughput()
                .WithSyncableContainerProperties();
        }).Configure<StockInventory>(itemBuilder =>
        {
            itemBuilder
                .WithContainer(CosmosConstants.Containers.StockInventoryContainer)
                .WithPartitionKey(CosmosConstants.PartitionKeys.StockInventoryPartitionKey)
                .WithManualThroughput()
                .WithSyncableContainerProperties();
        }).Configure<InventoryAudit>(itemBuilder =>
        {
            itemBuilder
                .WithContainer(CosmosConstants.Containers.InventoryAuditContainer)
                .WithPartitionKey(CosmosConstants.PartitionKeys.InventoryAuditPartitionKey)
                .WithManualThroughput()
                .WithSyncableContainerProperties()
                .WithContainerDefaultTimeToLive(TimeSpan.FromDays(90));
        }).Configure<ProductCategory>(itemBuilder =>
        {
            itemBuilder
                .WithContainer(CosmosConstants.Containers.ProductsCategoriesContainer)
                .WithPartitionKey(CosmosConstants.PartitionKeys.ProductsCategoriesPartitionKey)
                .WithManualThroughput()
                .WithSyncableContainerProperties();
        });
});

WebApplication app = builder.Build();

app.MapGet("/", () => "Services.Products");

app.MapPost("/products", async (CreateProduct command, IProductsService service) =>
{
    ServiceResult response = await service.CreateProduct(command);
    if (response.Success)
    {
        return Results.Ok(response);
    }

    return Results.BadRequest(response);
});

app.Run();