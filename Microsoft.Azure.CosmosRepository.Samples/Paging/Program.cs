using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Console;

ConfigurationBuilder configuration = new ConfigurationBuilder();

ServiceProvider provider = new ServiceCollection().AddCosmosRepository(options =>
{
    options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
    options.DatabaseId = "paging-db";
    options.ContainerPerItemType = true;
})
.AddSingleton<IConfiguration>(configuration.Build())
.BuildServiceProvider();

IRepository<Person> repository = provider.GetRequiredService<IRepository<Person>>();

await PageWithContinuationTokens();
await JumpStraightToPage();
// await JumpToStraightThenUseContinuationToken();






async Task JumpToStraightThenUseContinuationToken()
{
    int pageNumber = 4;
    int pageSize = 10;
    int totalItemsRead = 0;
    int totalItemsExpectedToRead = 60;
    string continuationToken = null;

    double totalRuCharge = 0;

    //TODO: this currently fails on the second pass when using the continuation token with a 400 bad request for a malformed token not entirely sure why yet.

    while (totalItemsRead < totalItemsExpectedToRead)
    {
        IPage<Person> page = await repository.PageAsync(p => p.Age > 10, pageSize: pageSize, page: pageNumber, continuationToken: continuationToken);
        continuationToken = page.ContinuationToken;
        totalItemsRead += page.Items.Count();
        pageNumber++;
        totalRuCharge += page.Charge;
        WriteLine($"Page {page.Number} returned {page.Items.Count()} results and cost {page.Charge} RUs");
        WriteLine($"Names in page [{string.Join(",", page.Items.Select(i => $"{i.Name} ({i.Age})"))}]");
        await Task.Delay(3000);
    }

    WriteLine($"Total charge for paging the whole container using continuation tokens {totalRuCharge} RUs");
}


async Task JumpStraightToPage()
{
    int pageNumber = 3;
    int pageSize = 10;

    IPage<Person> page = await repository.PageAsync(p => p.Age > 10, pageSize: pageSize, page: pageNumber);
    pageNumber++;
    WriteLine($"Page {page.Number} returned {page.Items.Count()} results and cost {page.Charge} RUs");
    WriteLine($"Names in page [{string.Join(",", page.Items.Select(i => $"{i.Name} ({i.Age})"))}]");
}



async Task PageWithContinuationTokens()
{
    int pageNumber = 1;
    int pageSize = 10;
    int totalItemsRead = 0;
    int totalItemsExpectedToRead = 100;
    string continuationToken = null;

    double totalRuCharge = 0;

    while (totalItemsRead < totalItemsExpectedToRead)
    {
        IPage<Person> page = await repository.PageAsync(p => p.Age > 10, pageSize: pageSize, page: pageNumber, continuationToken: continuationToken);
        continuationToken = page.ContinuationToken;
        totalItemsRead += page.Items.Count();
        pageNumber++;
        totalRuCharge += page.Charge;
        WriteLine($"Page {page.Number} returned {page.Items.Count()} results and cost {page.Charge} RUs");
        WriteLine($"Names in page [{string.Join(",", page.Items.Select(i => $"{i.Name} ({i.Age})"))}]");
    }

    WriteLine($"Total charge for paging the whole container using continuation tokens {totalRuCharge} RUs");
}

async Task Seed()
{
    Faker<Person> peopleFaker = new();
    peopleFaker
        .RuleFor(p => p.Name, f => f.Name.FullName())
        .RuleFor(p => p.Age, f => f.Random.Number(15, 45));

    List<Person> people = peopleFaker.Generate(100);
    await repository.CreateAsync(people);
}