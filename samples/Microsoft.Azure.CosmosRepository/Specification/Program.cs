// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Bogus;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Specification;

ConfigurationBuilder configuration = new();

ServiceProvider provider = new ServiceCollection().AddCosmosRepository(options =>
    {
        options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
        options.DatabaseId = "paging-db";
        options.ContainerPerItemType = true;
    })
    .AddSingleton<IConfiguration>(configuration.Build())
    .BuildServiceProvider();

IRepository<Person> repository = provider.GetRequiredService<IRepository<Person>>();

await SeedAsync();

SpecificationPagingSamples pagingExamples = new(repository);

Console.WriteLine("Specification paging");

await pagingExamples.BasicPageAsync();

Console.WriteLine("Specification continuation token");

await pagingExamples.BasicScrollingAsync();

SpecificationOrderSamples orderSamples = new(repository);

Console.WriteLine("Simple ordering");

await orderSamples.BasicOrderAsync();

Console.WriteLine("Multiple fields ordering");

//This requires a composite index with name desc then age to work
await orderSamples.MultipleOrderByAsync();

SpecificationFilterSamples filterSamples = new(repository);

Console.WriteLine("Simple filtering");

await filterSamples.FilterSamples();

FullSpecificationSamples fullSpecificationSamples = new(repository);

Console.WriteLine("Continuation Token with query sample");

await fullSpecificationSamples.FullContinuationTokenSpecificationAsync(10);

Console.WriteLine("Offset by page number query sample");

await fullSpecificationSamples.FullPageNumberSpecificationAsync(10);

async Task SeedAsync()
{
    IEnumerable<Person> current = await repository.GetAsync(x => x.Type == nameof(Person));

    if (current.Any())
    {
        return;
    }

    Faker<Person> peopleFaker = new();
    peopleFaker
        .RuleFor(p => p.Name, f => f.Name.FullName())
        .RuleFor(p => p.Age, f => f.Random.Number(15, 45));

    List<Person> people = peopleFaker.Generate(100);
    await repository.CreateAsync(people);
}