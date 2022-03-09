using System.Net;
using Bogus;
using CleanArchitecture;
using CleanArchitecture.Domain;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.CleanArchitecture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

ConfigurationBuilder configuration = new();

ServiceProvider provider = new ServiceCollection().AddCosmosRepository(options =>
    {
        options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
        options.DatabaseId = "clean-architecture-db";
        options.ContainerPerItemType = true;
        options.OptimizeBandwidth = false;
    })
    .AddSingleton<IConfiguration>(configuration.Build())
    .WithEtagMappedRepositories()
    .AddSingleton<IMapper<PersonItem, PersonEntity>, PersonMapper>()
    .AddTransient<IPersonRepository, PersonRepository>()
    .BuildServiceProvider();

IRepository<PersonItem> globalRepo = provider.GetRequiredService<IRepository<PersonItem>>();

await SeedAsync();

IEnumerable<PersonItem> people = await globalRepo.GetAsync(x => x.Type == nameof(PersonItem));
string personId = people.First().Id;

using (IPersonRepository repository = provider.GetRequiredService<IPersonRepository>())
{
    PersonEntity person = await repository.GetPerson(personId);

    Console.WriteLine($"Got {person.Name}: {person}");

    Console.WriteLine($"{person.Name} is having a birthday: {person}");
    person.Birthday();
    await repository.UpdatePerson(person);
    Console.WriteLine($"{person.Name} had a birthday: {person}");

    Console.WriteLine($"{person.Name} is having a birthday: {person}");
    person.Birthday();
    await repository.UpdatePerson(person);
    Console.WriteLine($"{person.Name} had a birthday: {person}");

    Console.WriteLine($"{person.Name} is having a birthday: {person}");
    person.Birthday();
    await repository.UpdatePerson(person);
    Console.WriteLine($"{person.Name} had a birthday: {person}");

    Console.WriteLine($"{person.Name} is having a birthday: {person}");
    person.Birthday();
    await repository.UpdatePerson(person);
    Console.WriteLine($"{person.Name} had a birthday: {person}");
}


async Task SeedAsync()
{
    IEnumerable<PersonItem> current = await globalRepo.GetAsync(x => x.Type == nameof(PersonItem));

    if (current.Any())
    {
        return;
    }

    Faker<PersonItem> peopleFaker = new();
    peopleFaker
        .RuleFor(p => p.Name, f => f.Name.FullName())
        .RuleFor(p => p.Age, f => f.Random.Number(15, 45));

    List<PersonItem> people = peopleFaker.Generate(100);
    await globalRepo.CreateAsync(people);
}