using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;
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
await orderSamples.MultipleOrderByAsync();

SpecificationFilterSamples filterSamples = new(repository);
Console.WriteLine("Simple filtering");
await filterSamples.FilterSamples();

FullSpecificationSamples fullSpecificationSamples = new(repository);

Console.WriteLine("Continuation Token with query sample");
await fullSpecificationSamples.FullContinuationTokenSpecificationAsync(10);

Console.WriteLine("Offset by page number query sample");
await fullSpecificationSamples.FullPageNumberSpecificationAsync(10);




async Task BasicScrollingAsync()
{
    double totalCharge = 0;

    IPage<Person> page = await repository.PageAsync(pageSize: 25, continuationToken: null);

    foreach (Person person in page.Items)
    {
        Console.WriteLine(person);
    }

    totalCharge += page.Charge;

    Console.WriteLine($"First 25 results cost {page.Charge}");

    page = await repository.PageAsync(pageSize: 25, continuationToken: page.Continuation);

    foreach (Person person in page.Items)
    {
        Console.WriteLine(person);
    }

    totalCharge += page.Charge;
    Console.WriteLine($"Second 25 results cost {page.Charge}");

    page = await repository.PageAsync(pageSize: 50, continuationToken: page.Continuation);

    foreach (Person person in page.Items)
    {
        Console.WriteLine(person);
    }

    totalCharge += page.Charge;

    Console.WriteLine($"Last 50 results cost {page.Charge}");
    Console.WriteLine($"Total Charge {totalCharge} RU's");
}



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