// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace Services.Products.Domain.Entities;

public class ProductCategory : Item
{
    protected override string GetPartitionKeyValue() =>
        Type;

    public string Name { get; set; }

    public string Description { get; set; }

    public ProductCategory(string name, string description)
    {
        Name = name;
        Description = description;
    }
}