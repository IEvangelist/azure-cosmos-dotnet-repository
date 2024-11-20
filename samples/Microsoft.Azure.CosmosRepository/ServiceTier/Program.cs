// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceTier;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, configuration) =>
    {
        configuration.Sources.Clear();
        configuration.AddCommandLine(args);
    })
    .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Debug))
    .ConfigureServices((context, services) =>
        services.AddCosmosRepository(options =>
        {
            options.ContainerId = "people-store";
            options.DatabaseId = "samples";
            options.OptimizeBandwidth = true;
            options.ContainerPerItemType = true;
        },
        clientOptions => clientOptions.AllowBulkExecution = true)
        .AddSingleton<IExampleService, ExampleService>()).Build();

await host.StartAsync();

// Demonstrate using the factory...
IRepositoryFactory factory =
    host.Services.GetService<IRepositoryFactory>()!;

// Demonstrate raw repo usage...
await Task.WhenAll(
    RoundtripCrudOpsOnPeopleRepositoryAsync(factory.RepositoryOf<Person>()),
    RawRepositoryExampleAsync(host.Services.GetService<IRepository<Widget>>()!));

// Demonstrate service wrapper around repo usage...
await ServiceExampleAsync(host.Services.GetService<IExampleService>()!);


static async Task RoundtripCrudOpsOnPeopleRepositoryAsync(IRepository<Person> repository)
{
    var maryShaw = new Person
    {
        FirstName = "Mary",
        LastName = "Shaw",
        BirthDate = new DateTime(1972, 7, 21)
    };
    var calvinWeatherfield = new Person
    {
        FirstName = "Calvin",
        LastName = "Weatherfield",
        BirthDate = new DateTime(1983, 2, 14)
    };

    // Creating...
    Console.WriteLine("[Person] Repository creating...");
    _ = await repository.CreateAsync([maryShaw, calvinWeatherfield]);

    // Reading...
    Person mary = await repository.GetAsync(maryShaw.Id, maryShaw.SyntheticPartitionKey);
    Person calvin = (await repository.GetAsync(p => p.BirthDate > new DateTime(1980, 1, 1))).Single();

    Console.WriteLine($"[Person] Read: {mary}");
    Console.WriteLine($"[Person] Read: {calvin}");

    // Updating...
    Console.WriteLine("[Person] Repository updating...");
    mary.BirthDate = new DateTime(1973, 7, 21); // Oops, Mary was actually born in 1973
    calvin.BirthDate = new DateTime(1982, 2, 14); // And Calvin was born in 1982...

    _ = repository.UpdateAsync(mary);
    _ = repository.UpdateAsync(calvin);

    // Read again / verify updates
    IEnumerable<Person> peopleWithoutMiddleNames = await repository.GetAsync(p => p.MiddleName == null);
    foreach (Person person in peopleWithoutMiddleNames)
    {
        Console.WriteLine($"[Person] Updated: {person}");
    }

    // Deleting...
    Console.WriteLine("[Person] Repository deleting...");
    await Task.WhenAll(new[]
    {
        repository.DeleteAsync(mary.Id, mary.SyntheticPartitionKey).AsTask(),
        repository.DeleteAsync(calvin.Id, calvin.SyntheticPartitionKey).AsTask()
    });
}

static async Task RawRepositoryExampleAsync(IRepository<Widget> repository)
{
    static async Task VerifyUpdates(IRepository<Widget> repo)
    {
        // Read again / verify updates
        IEnumerable<Widget> validWidgets = await repo.GetAsync(p => p.Name != null);
        foreach (Widget widget in validWidgets)
        {
            Console.WriteLine($"[Widget] Updated: {widget}");
        }
    }

    var widget1 = new Widget
    {
        Name = "Some fancy contraption",
        CreatedOrUpdatedOn = new DateTime(1984, 7, 7)
    };
    var widget2 = new Widget
    {
        Name = "The best telescope",
        CreatedOrUpdatedOn = new DateTime(1917, 4, 20)
    };

    // Creating...
    Console.WriteLine("[Widget] Repository creating...");
    _ = await repository.CreateAsync([widget1, widget2]);

    // Reading...
    Widget contraption = await repository.GetAsync(widget1.Id);
    Widget telescope = (await repository.GetByQueryAsync("SELECT * FROM w WHERE CONTAINS(w.Name, 'telescope')"))
        .Single();

    Console.WriteLine($"[Widget] Read: {contraption}");
    Console.WriteLine($"[Widget] Read: {telescope}");

    // Updating...
    Console.WriteLine("[Widget] Repository updating...");
    contraption.CreatedOrUpdatedOn = contraption.CreatedOrUpdatedOn.AddDays(1);
    telescope.CreatedOrUpdatedOn = telescope.CreatedOrUpdatedOn.AddDays(1);

    _ = repository.UpdateAsync(contraption);
    _ = repository.UpdateAsync(telescope);

    await VerifyUpdates(repository);

    // Reading ...
    QueryDefinition queryDefinition =
        new QueryDefinition("SELECT * FROM w WHERE CONTAINS(w.Name, @Name)").WithParameter("@Name", "telescope");
    telescope = (await repository.GetByQueryAsync(queryDefinition)).Single();


    Console.WriteLine($"[Widget] Read: {contraption}");
    Console.WriteLine($"[Widget] Read: {telescope}");

    // Updating...
    Console.WriteLine("[Widget] Repository updating...");
    contraption.CreatedOrUpdatedOn = contraption.CreatedOrUpdatedOn.AddDays(1);
    telescope.CreatedOrUpdatedOn = telescope.CreatedOrUpdatedOn.AddDays(1);

    _ = repository.UpdateAsync(contraption);
    _ = repository.UpdateAsync(telescope);

    await VerifyUpdates(repository);

    // Deleting...
    Console.WriteLine("[Widget] Repository deleting...");
    await Task.WhenAll(new[]
    {
        repository.DeleteAsync(contraption.Id).AsTask(),
        repository.DeleteAsync(telescope.Id).AsTask()
    });
}

static async Task ServiceExampleAsync(IExampleService service)
{
    var jamesBond = new Person
    {
        FirstName = "James",
        LastName = "Bond",
        BirthDate = new DateTime(1962, 3, 18)
    };
    var adeleGoldberg = new Person
    {
        FirstName = "Adele",
        LastName = "Goldberg",
        BirthDate = new DateTime(1945, 7, 22)
    };

    // Creating...
    Console.WriteLine("[Person] Service creating...");
    _ = await service.AddPeopleAsync(new[] { jamesBond, adeleGoldberg });

    // Reading...
    Person james = await service.ReadPersonByIdAsync(jamesBond.Id, jamesBond.SyntheticPartitionKey);
    Person adele = (await service.ReadPeopleAsync(p => p.LastName == "Goldberg")).Single();

    Console.WriteLine($"[Person] Read: {james}");
    Console.WriteLine($"[Person] Read: {adele}");

    // Updating...
    Console.WriteLine("[Person] Service updating...");
    james.BirthDate = new DateTime(1973, 7, 21); // Oops, Mary was actually born in 1973
    adele.BirthDate = new DateTime(1982, 2, 14); // And Calvin was born in 1982...

    _ = service.UpdatePersonAsync(james);
    _ = service.UpdatePersonAsync(adele);

    // Read again / verify updates
    IEnumerable<Person> peopleWithoutMiddleNames = await service.ReadPeopleAsync(p => p.MiddleName == null);
    foreach (Person person in peopleWithoutMiddleNames)
    {
        Console.WriteLine($"[Person] Updated: {person}");
    }

    // Deleting...
    Console.WriteLine("[Person] Service deleting...");
    await Task.WhenAll(
    [
        service.DeletePersonAsync(james).AsTask(),
        service.DeletePersonAsync(adele).AsTask()
    ]);
}
