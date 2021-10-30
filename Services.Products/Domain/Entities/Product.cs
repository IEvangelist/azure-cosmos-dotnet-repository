// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace Services.Products.Domain.Entities;

public class Product : Item
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string ProductCategoryId { get; set; }

    public Product(string name, string description, string imageUrl, string productCategoryId)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        ProductCategoryId = productCategoryId;
    }
}