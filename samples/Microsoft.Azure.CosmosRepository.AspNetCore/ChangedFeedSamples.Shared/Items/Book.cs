// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace ChangedFeedSamples.Shared.Items;

public class Book : FullItem
{
    public Book(string name, string author, string category)
    {
        Name = name;
        Author = author;
        Category = category;
        PartitionKey = category;
    }

    [JsonConstructor]
    private Book(string name, string author, string category, string partitionKey)
    {
        Name = name;
        Author = author;
        Category = category;
        PartitionKey = partitionKey;
    }

    public string Name { get; set; }

    public string Author { get; set; }

    public string Category { get; set; }

    public string PartitionKey { get; set; }

    public bool HasBeenUpdated { get; set; } = false;

    protected override string GetPartitionKeyValue() => PartitionKey;
}