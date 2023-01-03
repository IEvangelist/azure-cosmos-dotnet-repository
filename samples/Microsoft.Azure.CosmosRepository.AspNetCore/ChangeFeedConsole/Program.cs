// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using ChangedFeedSamples.Shared.Items;
using ChangedFeedSamples.Shared.Processors;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var connectionString = Environment.GetEnvironmentVariable("CosmosConnectionString") ??
                          throw new NullReferenceException("Make sure the connection string is set as an env var");

IServiceCollection services = new ServiceCollection();
ConfigurationBuilder configuration = new();

services.AddSingleton<IConfiguration>(configuration.Build());

services.AddCosmosRepository(options =>
{
    options.CosmosConnectionString = connectionString;
    options.DatabaseId = "change-feed-demo";
    options.ContainerPerItemType = true;

    options.ContainerBuilder.Configure<Book>(containerOptions =>
    {
        containerOptions.WithContainer("books");
        containerOptions.WithPartitionKey("/partitionKey");
        containerOptions.WithChangeFeedMonitoring();
    });

    options.ContainerBuilder.Configure<BookByIdReference>(containerOptions =>
    {
        containerOptions.WithContainer("books");
        containerOptions.WithPartitionKey("/partitionKey");
    });
});

services.AddSingleton<IItemChangeFeedProcessor<Book>, BookChangeFeedProcessor>();

IServiceProvider provider = services.BuildServiceProvider();

IChangeFeedService changeFeedService = provider.GetRequiredService<IChangeFeedService>();

CancellationTokenSource source = new();

await changeFeedService.StartAsync(source.Token);

IRepository<Book> bookRepository = provider.GetRequiredService<IRepository<Book>>();
IRepository<BookByIdReference> bookByIdReferenceRepository =
    provider.GetRequiredService<IRepository<BookByIdReference>>();

Book book = new("Book 1", "Mr Jones", "Tech");

await bookRepository.CreateAsync(book);

Console.WriteLine("Press any key to read the book by it's ID");
Console.ReadKey();

BookByIdReference bookByIdReference = await bookByIdReferenceRepository.GetAsync(book.Id);

Console.WriteLine($"Book category {bookByIdReference.Category}");

Console.WriteLine("Press any key to stop");
Console.ReadKey();

source.Cancel();