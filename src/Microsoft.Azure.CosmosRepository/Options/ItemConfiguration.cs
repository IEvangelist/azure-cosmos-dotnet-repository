// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Options;

internal class ItemConfiguration(
    Type type,
    string containerName,
    string partitionKeyPath,
    UniqueKeyPolicy? uniqueKeyPolicy,
    ThroughputProperties? throughputProperties,
    int defaultTimeToLive = -1,
    bool syncContainerProperties = false,
    ChangeFeedOptions? changeFeedOptions = null,
    bool useStrictTypeChecking = true)
{
    public Type Type { get; } = type;

    public string ContainerName { get; } = containerName;

    public string PartitionKeyPath { get; } = partitionKeyPath;

    public UniqueKeyPolicy? UniqueKeyPolicy { get; } = uniqueKeyPolicy;

    public ThroughputProperties? ThroughputProperties { get; } = throughputProperties;

    public int DefaultTimeToLive { get; } = defaultTimeToLive;

    public bool SyncContainerProperties { get; } = syncContainerProperties;

    public ChangeFeedOptions? ChangeFeedOptions { get; } = changeFeedOptions;

    public bool UseStrictTypeChecking { get; } = useStrictTypeChecking;
}