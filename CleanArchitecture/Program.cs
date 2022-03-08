using Bogus;
using CleanArchitecture;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.CleanArchitecture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

ConfigurationBuilder configuration = new();

ServiceProvider provider = new ServiceCollection().AddCosmosRepository(options =>
    {
        options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
        options.DatabaseId = "paging-db";
        options.ContainerPerItemType = true;
        options.OptimizeBandwidth = false;
    })
    .AddSingleton<IConfiguration>(configuration.Build())
    .WithEtagMappedRepositories()
    .AddSingleton<IItemMapper<PersonItem, PersonEntity>, PersonItemMapper>()
    .AddSingleton<IMappedItemMapper<PersonEntity, PersonItem>, PersonEntityMapper>()
    .BuildServiceProvider();

IRepository<PersonItem> globalRepo = provider.GetRequiredService<IRepository<PersonItem>>();

await SeedAsync();

await globalRepo.CreateAsync(new PersonItem()
{
    Name = "steve",
    Age = 23
});

using (IEtagMappedRepository<PersonItem, PersonEntity> repository = provider.GetRequiredService<IEtagMappedRepository<PersonItem, PersonEntity>>())
{
    PersonEntity person = await repository.GetAsync("steve");

    Console.WriteLine($"Got steve: {person} - {repository.Etag}");

    person.Birthday();
    await repository.UpdateAsync(person);

    Console.WriteLine($"Steve had a birthday: {person} - {repository.Etag}");
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