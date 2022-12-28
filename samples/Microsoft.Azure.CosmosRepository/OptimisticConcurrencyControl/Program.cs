// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OptimisticConcurrencyControl;


IRepository<BankAccount> BuildRepository(bool optimizeBandwidth)
{
    ConfigurationBuilder configuration = new();

    ServiceProvider provider = new ServiceCollection().AddCosmosRepository(options =>
        {
            options.CosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
            options.DatabaseId = "optimistic-concurrency-control";
            options.OptimizeBandwidth = optimizeBandwidth; // Must be false to receive the upto date etags back on update calls
            options.ContainerBuilder.Configure<BankAccount>(
                x => x.WithContainerDefaultTimeToLive(TimeSpan.FromHours(2)));
        })
        .AddSingleton<IConfiguration>(configuration.Build())
        .BuildServiceProvider();

    IRepository<BankAccount> repository = provider.GetRequiredService<IRepository<BankAccount>>();

    return repository;
}

Console.WriteLine("----------------------");
await ConcurrencyWithOptimizeBandwidthOff();
Console.WriteLine("----------------------");
await ConcurrencyWithOptimizeBandwidthOn();
Console.WriteLine("----------------------");
await ConcurrencyWithOptimizeBandwidthOnTrapDemo();
Console.WriteLine("----------------------");

async Task ConcurrencyWithOptimizeBandwidthOff()
{
    Console.WriteLine($"Optimized bandwidth OFF.");
    IRepository<BankAccount> repository = BuildRepository(false);

    BankAccount currentBankAccount = await repository.CreateAsync(new BankAccount()
    {
        Name = "Current Account",
        Balance = 500.0,
        TimeToLive = TimeSpan.FromHours(4)
    });

    Console.WriteLine($"Created a bank account: {currentBankAccount}.");

    currentBankAccount.Withdraw(500);
    await repository.UpdateAsync(currentBankAccount);
    currentBankAccount.Deposit(500);

    Console.WriteLine($"Simulating a withdrawal of 500 that happened from another service. Remote State - {await repository.GetAsync(currentBankAccount.Id)}. Local State - {currentBankAccount}");

    Console.WriteLine("Attempting to withdraw 250 from the bank account.");
    try
    {
        Console.WriteLine("Attempting to withdraw 250 from the bank account.");
        await repository.UpdateAsync(currentBankAccount.Id,
            builder => builder.Replace(account => account.Balance, currentBankAccount.Balance - 250), etag: currentBankAccount.Etag);
    }
    catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.PreconditionFailed)
    {
        Console.WriteLine("Failed to withdraw 250 from the bank account as the local state was out of date with the remote state.");
        Console.WriteLine("Syncing local state with remote state.");
        currentBankAccount = await repository.GetAsync(currentBankAccount.Id);
        Console.WriteLine($"Updated local state. {currentBankAccount}");
    }

    try
    {
        Console.WriteLine("Attempting to withdraw 750 from the bank account.");
        currentBankAccount.Withdraw(750);
        currentBankAccount = await repository.UpdateAsync(currentBankAccount);
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("Failed to withdraw 750 from the bank account as it would take the user overdrawn.");
    }

    currentBankAccount.Deposit(500);
    await repository.UpdateAsync(currentBankAccount);
    currentBankAccount.Withdraw(500);

    Console.WriteLine($"Simulating a deposit of 500 that happened from another service. Remote State - {await repository.GetAsync(currentBankAccount.Id)}. Local State - {currentBankAccount}");

    Console.WriteLine("Forcing the balance to equal 1000");
    currentBankAccount.Balance = 1000.0;
    currentBankAccount = await repository.UpdateAsync(currentBankAccount, ignoreEtag: true);

    Console.WriteLine($"Forced update succeeded as etag was ignored.  Remote State - {await repository.GetAsync(currentBankAccount.Id)}. Local State - {currentBankAccount}");
}

async Task ConcurrencyWithOptimizeBandwidthOn()
{
    Console.WriteLine($"Optimized bandwidth ON.");
    IRepository<BankAccount> repository = BuildRepository(true);

    BankAccount bankAccountInfo = await repository.CreateAsync(new BankAccount()
    {
        Name = "Current Account",
        Balance = 500.0,
        TimeToLive = TimeSpan.FromSeconds(-1)
    });

    await repository.CreateAsync(bankAccountInfo);

    BankAccount currentBankAccount = await repository.GetAsync(bankAccountInfo.Id);
    Console.WriteLine($"Created a bank account: {currentBankAccount}.");

    currentBankAccount.Withdraw(500);
    await repository.UpdateAsync(currentBankAccount);
    currentBankAccount.Deposit(500);

    Console.WriteLine($"Simulating a withdrawal of 500 that happened from another service. Remote State - {await repository.GetAsync(currentBankAccount.Id)}. Local State - {currentBankAccount}");

    try
    {
        currentBankAccount.Withdraw(250.0);
        Console.WriteLine("Attempting to withdraw 250 from the bank account.");
        await repository.UpdateAsync(currentBankAccount.Id,
            builder => builder.Replace(account => account.Balance, currentBankAccount.Balance - 250), etag: currentBankAccount.Etag);
        currentBankAccount = await repository.GetAsync(bankAccountInfo.Id);
    }
    catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.PreconditionFailed)
    {
        Console.WriteLine("Failed to withdraw 250 from the bank account as the local state was out of date with the remote state.");
        Console.WriteLine("Syncing local state with remote state.");
        currentBankAccount = await repository.GetAsync(currentBankAccount.Id);
        Console.WriteLine($"Updated local state. {currentBankAccount}");

    }

    Console.WriteLine("Attempting to withdraw 750 from the bank account.");
    try
    {
        currentBankAccount.Withdraw(750.0);
        Console.WriteLine("Attempting to withdraw 750 from the bank account.");
        await repository.UpdateAsync(currentBankAccount);
        currentBankAccount = await repository.GetAsync(bankAccountInfo.Id);
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("Failed to withdraw 750 from the bank account as it would take the user overdrawn.");
    }

    currentBankAccount.Deposit(500);
    await repository.UpdateAsync(currentBankAccount);
    currentBankAccount.Withdraw(500);

    Console.WriteLine($"Simulating a deposit of 500 that happened from another service. Remote State - {await repository.GetAsync(currentBankAccount.Id)}. Local State - {currentBankAccount}");

    Console.WriteLine("Forcing the balance to equal 1000");
    currentBankAccount.Balance = 1000.0;
    await repository.UpdateAsync(currentBankAccount, ignoreEtag: true);
    currentBankAccount = await repository.GetAsync(bankAccountInfo.Id);

    Console.WriteLine($"Forced update succeeded as etag was ignored.  Remote State - {await repository.GetAsync(currentBankAccount.Id)}. Local State - {currentBankAccount}");
}

// Be cautious if doing multiple updates and using the result from the update.
// If optimize bandwidth is on then the result returned will be the value passed in and the etag will not update.
async Task ConcurrencyWithOptimizeBandwidthOnTrapDemo()
{
    Console.WriteLine($"Optimized bandwidth ON.");
    IRepository<BankAccount> repository = BuildRepository(true);

    var bankAccountInfo = new BankAccount()
    {
        Name = "Current Account",
        Balance = 500.0
    };

    BankAccount currentBankAccount = await repository.CreateAsync(bankAccountInfo);
    Console.WriteLine($"Created a bank account: {currentBankAccount}. From information in {bankAccountInfo}");

    Console.WriteLine($"Setting balance to 200");
    currentBankAccount.Balance = 200;
    currentBankAccount = await repository.UpdateAsync(currentBankAccount);
    Console.WriteLine($"Updated bank account: {currentBankAccount}.");

    Console.WriteLine($"Setting balance to 300. Using result from last update.");
    currentBankAccount.Balance = 300;
    try
    {
        currentBankAccount = await repository.UpdateAsync(currentBankAccount);
        Console.WriteLine($"Updated bank account: {currentBankAccount}.");
    }
    catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.PreconditionFailed)
    {
        Console.WriteLine("Failed to update balance as the etags did not match.");
    }

    Console.WriteLine($"Reattempting using value from the database.");
    currentBankAccount = await repository.GetAsync(currentBankAccount.Id);
    currentBankAccount.Balance = 300;
    await repository.UpdateAsync(currentBankAccount);
    Console.WriteLine($"Updated bank account: {await repository.GetAsync(currentBankAccount.Id)}.");

}
