// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public class RatingByCategory : FullItem
{
    public string ProductId { get; }

    public string PartitionKey { get; }

    public int Stars { get; }

    public string Text { get; }
    public string CategoryId { get; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public RatingByCategory(string productId, int stars, string text, string categoryId)
    {
        ProductId = productId;
        PartitionKey = categoryId;
        Stars = stars;
        Text = text;
        CategoryId = categoryId;
    }

    [JsonConstructor]
    protected RatingByCategory(string productId, string partitionKey, int stars, string text, string categoryId)
    {
        ProductId = productId;
        PartitionKey = categoryId;
        Stars = stars;
        Text = text;
        CategoryId = categoryId;
    }
}