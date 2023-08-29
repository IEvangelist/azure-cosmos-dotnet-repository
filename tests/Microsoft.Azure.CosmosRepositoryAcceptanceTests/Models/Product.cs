// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public class Product : FullItem
{
    public string Name { get; set; }

    public string CategoryId { get; set; }

    public string PartitionKey { get; set; }

    public double Price { get; set; }

    public StockInformation Stock { get; set; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public void ApplySaleDiscount(double salePercentage) =>
        Price -= Price * salePercentage;

    public Product(string name, string categoryId, double price, StockInformation stock)
    {
        Name = name;
        CategoryId = categoryId;
        Stock = stock;
        Price = price;
        PartitionKey = categoryId;
    }

    [JsonConstructor]
    private Product(string name, string categoryId, string partitionKey, double price, StockInformation stock)
    {
        Name = name;
        CategoryId = categoryId;
        PartitionKey = partitionKey;
        Price = price;
        Stock = stock;
    }
}