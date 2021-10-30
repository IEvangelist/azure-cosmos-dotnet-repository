// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Services.Products.Infrastructure.Cosmos;

public static class CosmosConstants
{
    public static class Containers
    {
        public const string ProductsCategoriesContainer = "categories";
        public const string ProductsContainer = "products";
        public const string StockInventoryContainer = "inventory";
        public const string InventoryAuditContainer = "inventory-audit";
    }

    public static class PartitionKeys
    {
        public const string ProductsCategoriesPartitionKey = "/type";
        public const string ProductsPartitionKey = "/productCategoryId";
        public const string StockInventoryPartitionKey = "/productCategoryId";
        public const string InventoryAuditPartitionKey = "/productId";
    }
}