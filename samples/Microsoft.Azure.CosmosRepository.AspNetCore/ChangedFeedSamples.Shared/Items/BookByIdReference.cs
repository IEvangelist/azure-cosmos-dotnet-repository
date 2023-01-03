// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace ChangedFeedSamples.Shared.Items;

public class BookByIdReference : Item
{
    public BookByIdReference(string id, string category)
    {
        Id = id;
        Category = category;
        PartitionKey = id;
    }

    [JsonConstructor]
    private BookByIdReference(string id, string category, string partitionKey)
    {
        Id = id;
        Category = category;
        PartitionKey = partitionKey;
    }

    public string Category { get; set; }

    public string PartitionKey { get; set; }
}