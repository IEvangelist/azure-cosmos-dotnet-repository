using System.Net;
using Bogus;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OptimisticConcurrencyControl;

ConfigurationBuilder configuration = new();

ServiceProvider provider = new ServiceCollection().AddCosmosRepository(options =>
    {
        options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
        options.DatabaseId = "optimistic-concurrency-control";
        options.OptimizeBandwidth = false;
    })
    .AddSingleton<IConfiguration>(configuration.Build())
    .BuildServiceProvider();

IRepository<BankAccount> repository = provider.GetRequiredService<IRepository<BankAccount>>();

await SeedAsync();
await Concurrency();

async Task Concurrency()
{
    double totalCharge = 0;

    BankAccount createdBankAccount = await repository.CreateAsync(new BankAccount()
    {
        Name = "Current Account",
        Balance = 500.0
    });

    Console.WriteLine($"Created a bank account: {createdBankAccount}.");
    Console.WriteLine("Attempting to update the same instance of the created bank account three times.");

    Console.WriteLine($"1) Attempting to update balance to 750.0.");
    BankAccount bankAccountUpdate = await repository.GetAsync(createdBankAccount.Id);
    bankAccountUpdate.Balance = 750.0;
    BankAccount updatedBankAccount = await repository.UpdateAsync(bankAccountUpdate);

    Console.WriteLine($"1) Bank account updated {updatedBankAccount}.");

    try
    {
        bankAccountUpdate.Balance = 1000.0;
        Console.WriteLine($"2) Attempting to update balance to 1000.0 using the original etag. {bankAccountUpdate}");
        await repository.UpdateAsync(bankAccountUpdate);
    }
    catch (CosmosException exception)
    {
        if (exception.StatusCode == HttpStatusCode.PreconditionFailed)
        {
            Console.WriteLine("2) Failed to update balance to 1000.0 as etags did not match.");
        }
    }

    Console.WriteLine($"3) Forcing balance to update update balance to 1000.0 using the original etag. But explicitly telling it to ignore the etag {bankAccountUpdate}");
    updatedBankAccount = await repository.UpdateAsync(bankAccountUpdate, ignoreEtag: true);
    Console.WriteLine($"3) Bank account updated {updatedBankAccount}.");

    BankAccount finalAccountInfo = await repository.GetAsync(createdBankAccount.Id);
    Console.WriteLine($"Final account information {finalAccountInfo}");
}

async Task SeedAsync()
{
    IEnumerable<BankAccount> current = await repository.GetAsync(x => x.Type == nameof(BankAccount));

    if (current.Any())
    {
        return;
    }

    Faker<BankAccount> peopleFaker = new();
    peopleFaker
        .RuleFor(p => p.Name, f => f.Name.FullName())
        .RuleFor(p => p.Balance, f => f.Random.Number(0, 10000));

    List<BankAccount> people = peopleFaker.Generate(100);
    await repository.CreateAsync(people);
}