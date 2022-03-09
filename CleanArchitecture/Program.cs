using System.Net;
using Bogus;
using CleanArchitecture;
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
    .BuildServiceProvider();

IRepository<PersonItem> globalRepo = provider.GetRequiredService<IRepository<PersonItem>>();

await SeedAsync();

IEnumerable<PersonItem> people = await globalRepo.GetAsync(x => x.Type == nameof(PersonItem));
string personId = people.First().Id;

using (IEtagMappedRepository<PersonItem, PersonEntity> repository = provider.GetRequiredService<IEtagMappedRepository<PersonItem, PersonEntity>>())
{
    PersonEntity person = await repository.GetAsync(personId);

    Console.WriteLine($"Got {person.Name}: {person} - {repository.GetEtag(personId)}");

    Console.WriteLine($"{person.Name} is having a birthday: {person} - {repository.GetEtag(personId)}");
    person.Birthday();
    await repository.UpdateAsync(person);
    Console.WriteLine($"{person.Name} had a birthday: {person} - {repository.GetEtag(personId)}");

    Console.WriteLine($"{person.Name} is having a birthday: {person} - {repository.GetEtag(personId)}");
    person.Birthday();
    await repository.UpdateAsync(person);
    string oldEtag = repository.GetEtag(personId) ?? throw new NullReferenceException("Shouldn't be null");
    Console.WriteLine($"{person.Name} had a birthday: {person} - {repository.GetEtag(personId)}");

    Console.WriteLine($"{person.Name} is having a birthday: {person} - {repository.GetEtag(personId)}");
    person.Birthday();
    await repository.UpdateAsync(person);
    Console.WriteLine($"{person.Name} had a birthday: {person} - {repository.GetEtag(personId)}");

    try
    {
        //TODO: Remove
        (repository as EtagMappedRepository<PersonItem, PersonEntity>)!.ForceEtag(personId, oldEtag);
        Console.WriteLine($"{person.Name} is having a birthday: {person} - {repository.GetEtag(person.Id)}");
        person.Birthday();
        await repository.UpdateAsync(person);
        Console.WriteLine($"{person.Name} had a birthday: {person} - {repository.GetEtag(person.Id)}");
    }
    catch (CosmosException e) when (e.StatusCode == HttpStatusCode.PreconditionFailed)
    {
        Console.WriteLine($"{person.Name} failed to have a birthday as the etag was out of date: {person} - {repository.GetEtag(person.Id)}");
    }
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