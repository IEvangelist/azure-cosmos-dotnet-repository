using System.Reflection;
using ChangeFeedConsole;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection services = new ServiceCollection();

void CosmosRepositoryConfiguration(RepositoryOptions options)
{
    options.DatabaseId = "change-feed-console-demo";
    options.ContainerPerItemType = true;
    options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");

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
}

services.AddCosmosRepository(CosmosRepositoryConfiguration)
    .AddCosmosRepositoryItemChangeFeedProcessors(Assembly.GetExecutingAssembly());

IServiceProvider provider = services.BuildServiceProvider();

IChangeFeedService changeFeedService = provider.GetRequiredService<IChangeFeedService>();

await changeFeedService.StartAsync(default);

IRepository<Book> bookRepository = provider.GetRequiredService<IRepository<Book>>();
IRepository<BookByIdReference> bookByIdReferenceRepository = provider.GetRequiredService<IRepository<BookByIdReference>>();

Book book = new("Book 1", "Mr Jones", "Tech");

await bookRepository.CreateAsync(book);

//Wait for the change feed to be triggered
await Task.Delay(5000);

BookByIdReference bookByIdReference = await bookByIdReferenceRepository.GetAsync(book.Id);

Console.WriteLine(bookByIdReference);

Console.WriteLine("Press any key to stop");
Console.ReadKey();

await changeFeedService.StopAsync(default);