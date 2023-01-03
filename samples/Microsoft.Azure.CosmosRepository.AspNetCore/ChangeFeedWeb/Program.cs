// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using ChangedFeedSamples.Shared;
using ChangedFeedSamples.Shared.Items;
using ChangedFeedSamples.Shared.Processors;
using ChangeFeedWeb.Routes;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.ChangeFeed;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCosmosRepository(options =>
{
    options.DatabaseId = "change-feed-demo";
    options.ContainerPerItemType = true;

    options.ContainerBuilder.Configure<Book>(containerOptions =>
    {
        containerOptions.WithContainer(BookDemoConstants.Container);
        containerOptions.WithPartitionKey(BookDemoConstants.PartitionKey);
        containerOptions.WithChangeFeedMonitoring();
    });

    options.ContainerBuilder.Configure<BookByIdReference>(containerOptions =>
    {
        containerOptions.WithContainer(BookDemoConstants.Container);
        containerOptions.WithPartitionKey(BookDemoConstants.PartitionKey);
    });
});

builder.Services.AddSingleton<IItemChangeFeedProcessor<Book>, BookChangeFeedProcessor>();

builder.Services.AddCosmosRepositoryChangeFeedHostedService();
builder.Services.AddCosmosRepositoryItemChangeFeedProcessors(typeof(BookChangeFeedProcessor).Assembly);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.UseSwagger();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapBookRoutes();

app.UseSwaggerUI();

app.Run();