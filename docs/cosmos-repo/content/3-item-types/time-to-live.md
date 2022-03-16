---
title: "TimeToLiveItem"
weight: 4
chapter: true
pre: "<b>3. </b>"
---

## Time To Live

The time to live property can be set at both an item and container level. At a container level this can be done through the container options builder:

```csharp
options.ContainerBuilder.Configure<BankAccount>(
  x => x.WithContainerDefaultTimeToLive(TimeSpan.FromHours(2)));
```

In the above example the container would have a default item lifespan of 2 hours. This can be overriden at the item level by using the `TimeToLive` property when correctly implemented. This is available through the `FullItem` and `TimeToLiveItem` base classes. The example below shows this been overriden so the item has a lifespan of 4 hours rather than the default of 2:

```csharp
BankAccount currentBankAccount = await repository.CreateAsync(
  new BankAccount()
    {
        Name = "Current Account",
        Balance = 500.0,
        TimeToLive = TimeSpan.FromHours(4)
    });
```

If you didn't want that specific item to ever expire the following code can be used:

```csharp
BankAccount currentBankAccount = await repository.CreateAsync(
  new BankAccount()
    {
        Name = "Current Account",
        Balance = 500.0,
        TimeToLive = TimeSpan.FromSeconds(-1)
    });
```

The demo `BankAccount` class can be found in the [OptimisticCurrencyControl sample](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/Microsoft.Azure.CosmosRepository.Samples/OptimisticConcurrencyControl) and its implementation looks like the following:

```csharp
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace OptimisticConcurrencyControl;

[Container("accounts")]
[PartitionKeyPath("/id")]
public class BankAccount : FullItem
{
    public string Name { get; set; } = string.Empty;
    public double Balance { get; set; }

    public void Withdraw(double amount)
    {
        if (Balance - amount < 0.0) throw new InvalidOperationException("Cannot go overdrawn");

        Balance -= amount;
    }

    public void Deposit(double amount)
    {
        Balance += amount;
    }

    public override string ToString() =>
        $"Account (Name = {Name}, Balance = {Balance}, Etag = {Etag})";
}
```

This [page](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/time-to-live) goes into more detail about the various combinations.