// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public class Rating : FullItem
{
    public string ProductId { get; set; }

    public string PartitionKey { get; set; }

    public int Stars { get; set; }

    public string Text { get; set; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public Rating(string productId, int stars, string text)
    {
        ProductId = productId;
        PartitionKey = ProductId;
        Stars = stars;
        Text = text;
    }

    [JsonConstructor]
    private Rating(string productId, string partitionKey, int stars, string text)
    {
        ProductId = productId;
        PartitionKey = partitionKey;
        Stars = stars;
        Text = text;
    }
}