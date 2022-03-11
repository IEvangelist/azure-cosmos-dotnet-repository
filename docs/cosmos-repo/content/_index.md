# Azure Cosmos DB Repository .NET SDK

This package wraps the [NuGet: Microsoft.Azure.Cosmos package](https://www.nuget.org/packages/Microsoft.Azure.Cosmos),
exposing a simple dependency-injection enabled `IRepository<T>` interface.

![Cosmos Repository](https://raw.githubusercontent.com/IEvangelist/azure-cosmos-dotnet-repository/main/CosmosRepository.png)

## Overview

The repository is responsible for all of the create, read, update, and delete (CRUD) operations on objects `where T : Item`. The `Item` type adds
several properties, one which is a globally unique identifier defined as:

```csharp
[JsonProperty("id")]
public string Id { get; set; } = Guid.NewGuid().ToString();
```

Additionally, a type property exists which indicates the subclass name (this is used for filtering implicitly on your behalf):

```csharp
[JsonProperty("type")]
public string Type { get; set; }
```

Finally, a partition key property is used internally to manage partitioning on your behalf. This can optionally be overridden on an item per item basis.

📣 [Azure Cosmos DB - Official Blog](https://devblogs.microsoft.com/cosmosdb/azure-cosmos-db-repository-net-sdk-v-1-0-4)

For more information, see [Customizing configuration sources](https://docs.microsoft.com/azure/azure-functions/functions-dotnet-dependency-injection?WC.m_id=dapine#customizing-configuration-sources).

#### Authenticating using an identity

The Azure Cosmos DB .NET SDK also supports authentication using identities, which are considered superior from an audit and granularity of permissions perspective. Authenticating using a connection string essentially provides full access to perform operations within the [data plane](https://docs.microsoft.com/en-us/azure/cosmos-db/role-based-access-control)
of your Cosmos DB Account. More information on the Azure control plane and data plane is available [here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/control-plane-and-data-plane).

This libary also supports authentication using an identity. To authenticate using an identity (User, Group, Application Registration, or Managed Identity) you will need to set the `AccountEndpoint` and `TokenCredential` options that are available on the `RepositoryOptions` class.

In a basic scenario, there are three steps that need to be completed:

1. If the identity that you would like to use, does not exist in Azure Active Directory, create it now.

1. Use the Azure CLI to [assign](https://docs.microsoft.com/en-us/cli/azure/cosmosdb/sql/role/assignment?view=azure-cli-latest#az_cosmosdb_sql_role_assignment_create) the appropriate role to your identity at the desired scope. - In most cases, using the [built-in roles](https://docs.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac#built-in-role-definitions) will be sufficient. However, there is support for creating custom role definitions using the Azure CLI, you can read more on this [here](https://docs.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac#role-definitions).
1. Configure your application using the `AddCosmosRepository` method in your `Startup.cs` file:

   ```csharp
   using Azure.Identity;

   public void ConfigureServices(IServiceCollection services)
   {
       DefaultAzureCredential credential = new();

       services.AddCosmosRepository(
           options =>
           {
               options.TokenCredential = credential;
               options.AccountEndpoint = "< account endpoint URI >";
               options.ContainerId = "data-store";
               options.DatabaseId = "samples";
           });
   }
   ```

The example above is using the `DefaultAzureCredential` object provided by the [Azure Identity](https://www.nuget.org/packages/Azure.Identity) NuGet package, which provides seamless integration with Azure Active Directory. More information on this package is available [here](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme).

## Advanced partitioning strategy

As a consumer of Azure Cosmos DB, you can choose how to partition your data. By default, this repository SDK will partition items using their `Item.Id` value as the `/id` partition in the storage container. However, you can override this default behavior by:

1. Declaratively specifying the partition key path with `PartitionKeyPathAttribute`
1. Override the `Item.GetPartitionKeyValue()` method
1. Ensure the the property value of the composite or synthetic key is serialized to match the partition key path
1. Set `RepositoryOptions__ContainerPerItemType` to `true`, to ensure that your item with explicit partitioning is correctly maintained

As an example, considering the following:

```csharp
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;
using System;

namespace Example
{
    [PartitionKeyPath("/synthetic")]
    public class Person : Item
    {
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;

        [JsonProperty("synthetic")]
        public string SyntheticPartitionKey =>
            $"{FirstName}-{LastName}"; // Also known as a "composite key".

        protected override string GetPartitionKeyValue() => SyntheticPartitionKey;
    }
}
```

<!--
Notes for tagging releases:
  https://rehansaeed.com/the-easiest-way-to-version-nuget-packages/#minver

git tag -a 2.1.3 -m "Build v2.1.3"
git push upstream --tags
dotnet build
-->

## In-memory Repository

This library also includes an in-memory version of `IRepository<T>`. To use it swap out the normal
`services.AddCosmosRepository()` for
`services.AddInMemoryCosmosRepository()` and have all of your items stored in memory. This is a great tool for running integration tests using a package such as `Microsoft.AspNetCore.Mvc.Testing`, and not having to incur the cost of data stored in an Azure Cosmos DB resource.

## Optimistic Concurrency Control with Etags

The default repository now supports etags and will pass them when `IItemWithEtag` is implemented correctly or the base classes `EtagItem` or `FullItem` are used. The etag check is enforced on all updates when `TItem` is of the correct type. It can however be bypassed by setting the `ignoreEtag` optional parameter in the relevant async methods. The InMemory repository also supports OCC with Etags. The [OptimisticCurrencyControl sample](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/Microsoft.Azure.CosmosRepository.Samples/OptimisticConcurrencyControl) shows these features.

### Getting the new etag

When creating a new object, if storing in memory, it is important to store the result from the create call to ensure you have the correct etag for future updated.

For example your code should look something like this:

```csharp
TItem currentItem = new TItem(...);
currentItem = _repository.CreateAsync(currentItem);
```

### Sequential updates

When doing sequential updates to the same item it is important to use the result from the update method (when OptimizeBandwith is false) or refetch the updated data each time (when OptimizeBandwith is true) otherwise the etag value will not be updated. The following code shows what to do in each case:

#### Optimize Bandwith Off

```csharp
TItem currentItem = _repository.CreateAsync(itemConfig);
currentItem = _repository.UpdateAsync(currentItem);
currentItem = _repository.UpdateAsync(currentItem);
```

#### Optimize Bandwith On

```csharp
TItem currentItem = _repository.CreateAsync(itemConfig);
_repository.UpdateAsync(currentItem);
currentItem = _repository.GetAsync(currentItem.Id);
_repository.UpdateAsync(currentItem);
currentItem = _repository.GetAsync(currentItem);
currentItem = _repository.UpdateAsync(currentItem);
```

### Catching mismatched etag errors

The following code shows how to catch the error when the etags do not match.

```csharp
  try
  {
      currentBankAccount = await repository.UpdateAsync(currentBankAccount);
      Console.WriteLine($"Updated bank account: {currentBankAccount}.");
  }
  catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.PreconditionFailed)
  {
      Console.WriteLine("Failed to update balance as the etags did not match.");
  }
```

### Ignoring the etag

The following code shows how to ignore the etag when doing an update.

```csharp
await repository.UpdateAsync(currentBankAccount, ignoreEtag: true);
```

### Passing the etag to a patch update

The following code shows how to pass the etag when doing a update to specific properties.

```csharp
await repository.UpdateAsync(currentBankAccount.Id,
  builder => builder.Replace(account => account.Balance, currentBankAccount.Balance - 250), etag: currentBankAccount.Etag);
```

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

## Created and Last Updated

The last updated value is retrieved from the \_ts property that Cosmos DB sets; as documented [here](https://docs.microsoft.com/en-us/rest/api/cosmos-db/documents#:~:text=the%20document%20resource.-,_ts,updated%20timestamp%20of%20the%20resource.%20The%20value%20is%20a%20timestamp.,-_self). This property is deserialised and is available in the raw seconds (`LastUpdatedTimeRaw`) since epoch and a human readable format (`LastUpdatedTimeUtc`). Both the base classes `FullItem` and `TimeStampedItem` contain these properties.

The `CreatedTimeUtc` time property available in both the base classes `FullItem` and `TimeStampedItem` is set when `CreateAsync` is called on the repository. However, this property can be set prior to calling `CreateAsync` in which case it wont be overwritten; allowing you to set your own `CreatedTimeUtc` value. This does mean that when using existing date the `CreatedTimeUtc` property will be null.

## Samples

Visit the `Microsoft.Azure.CosmosRepository.Samples` [directory](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/Microsoft.Azure.CosmosRepository.Samples) for samples on how to use the library with:

- [Azure Functions](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier)
- [Services](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier)
- [Controllers (web apps)](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/Microsoft.Azure.CosmosRepository.Samples/WebTier)
- [Paging](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/Microsoft.Azure.CosmosRepository.Samples/Paging)
## Deep-dive video

[![A deep dive into the Azure Cosmos DB repository pattern NET SDK](https://raw.githubusercontent.com/IEvangelist/azure-cosmos-dotnet-repository/main/images/deep-dive-talk.png)](https://www.youtube.com/watch?v=izdnmBrTweA)

## Discord

Get extra support on our dedicated Discord channel.

[![alt Join the conversation](https://img.shields.io/discord/868239483529723914.svg "Discord")](https://discord.com/invite/qMXrX4shAv)
