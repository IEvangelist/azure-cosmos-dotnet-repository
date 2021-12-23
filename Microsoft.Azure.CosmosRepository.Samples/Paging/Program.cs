using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
await BasicScrollingAsync();
await BasicPageAsync();

async Task BasicPageAsync()
{
    double totalCharge = 0;
    IPage<Person> page = await repository.PageAsync(pageNumber:1,pageSize: 25);
    while (page.HasNextPage)
    {
        foreach (Person person in page.Items)
        {
            Console.WriteLine(person);
        }
        totalCharge += page.Charge;
        Console.WriteLine($"Get page {page.PageNumber} 25 results cost {page.Charge}");
    }
    Console.WriteLine($"Total Charge {totalCharge} RU's");
}

async Task BasicScrollingAsync()
{
    double totalCharge = 0;

    IPage<Person> page = await repository.PageAsync(pageSize: 25, continuationToken: string.Empty);

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