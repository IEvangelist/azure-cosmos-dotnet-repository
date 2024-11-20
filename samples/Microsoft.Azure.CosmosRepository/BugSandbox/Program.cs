// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Specification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// https://learn.microsoft.com/azure/cosmos-db/how-to-develop-emulator?tabs=docker-linux%2Ccsharp&pivots=api-nosql
const string EmulatorConnectionString = """
    AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;
    """;

var builder = Host.CreateApplicationBuilder(args);

ServiceProvider provider = builder.Services.AddCosmosRepository(options =>
    {
        options.CosmosConnectionString = EmulatorConnectionString;
        options.DatabaseId = "bug-sandbox";
        options.ContainerBuilder.Configure<ExampleModel>(
            static containerOptions => containerOptions
                .WithoutStrictTypeChecking()
                .WithContainer("Example")
                .WithPartitionKey("/id"));
    })
    .BuildServiceProvider();

IRepository<ExampleModel> repository = provider.GetRequiredService<IRepository<ExampleModel>>();

await SeedAsync();

var queryResult = await repository.QueryAsync(new ExampleSpecification());

if (queryResult is { })
{
    // Noice!
}

async Task SeedAsync()
{
    IEnumerable<ExampleModel> current = await repository.GetAsync(x => x.Type == nameof(ExampleModel));

    if (current.Any())
    {
        return;
    }

    await repository.CreateAsync(
        [
            new() { Category = "Red" },
            new() { Category = "Yellow" },
            new() { Category = "Blue" },
            new() { Category = "Orange" },
            new() { Category = "Green" },
        ]);
}

public class ExampleModel : Item
{
    public required string Category { get; set; }
}

public class ExampleSpecification : DefaultSpecification<ExampleModel>
{
    public ExampleSpecification() =>
        Query.Where(q => q.Category == "Red");
}