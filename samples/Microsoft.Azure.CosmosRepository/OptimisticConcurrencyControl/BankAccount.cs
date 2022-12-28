// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

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

    public void Deposit(double amount) => Balance += amount;

    public override string ToString() =>
        $"Account (Name = {Name}, Balance = {Balance}, Etag = {Etag})";
}