using ChangedFeedSamples.Shared;
using ChangedFeedSamples.Shared.Items;
using ChangedFeedSamples.Shared.Processors;
using ChangeFeedWeb.Routes;
using ChangeFeedWeb.Services;

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

builder.Services.AddCosmosRepositoryItemChangeFeedProcessorsFromAssemblyContainingType<BookChangeFeedProcessor>();

builder.Services.AddHostedService<ChangeFeedHostedService>();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.UseSwagger();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapBookRoutes();

app.UseSwaggerUI();

app.Run();