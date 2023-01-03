// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

[Container("people")]
[PartitionKeyPath("/id")]
public class Person : Item
{
    public string Name { get; set; }

    public int Age { get; set; }

    public override string ToString() =>
        $"Person (Name = {Name}, Age = {Age})";
}