---
title: "ETagItem"
weight: 2
chapter: true
pre: "<b>1. </b>"
---

## Optimistic Concurrency Control with ETags

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