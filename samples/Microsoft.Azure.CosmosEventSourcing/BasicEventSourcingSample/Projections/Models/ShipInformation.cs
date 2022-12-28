// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Projections.Models;

public class ShipInformation : FullItem
{
    public string Name { get; init; }
    public DateTime Commissioned { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? LatestPort { get; set; }

    public double? LatestCargoWeight { get; set; }
    public string PartitionKey { get; set; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public ShipInformation(
        string name,
        DateTime commissioned,
        DateTime createdAt)
    {
        Id = name;
        Name = name;
        Commissioned = commissioned;
        CreatedAt = createdAt;
        PartitionKey = Type;
    }
}