// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace BasicEventSourcingSample.Projections.Models;

public class ShipInformation : FullItem
{
    public string Name { get; init; }

    public DateTime CreatedAt { get; set; }

    public ShipInformation Information { get; set; }

    public string? CurrentPort { get; set; }

    public double? CurrentCargoWeight { get; set; }
    public string PartitionKey { get; set; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public ShipInformation(string name, DateTime createdAt, ShipInformation information)
    {
        Name = name;
        CreatedAt = createdAt;
        Information = information;
        PartitionKey = Type;
    }
}