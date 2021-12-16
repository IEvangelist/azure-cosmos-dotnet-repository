// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace OptimisticConcurrencyControl;

[Container("accounts")]
[PartitionKeyPath("/id")]
public class BankAccount : EtagItem
{
    public string Name { get; set; }
    public double Balance { get; set; }

    public override string ToString() =>
        $"Account (Name = {Name}, Balance = {Balance}, Etag = {Etag})";
}