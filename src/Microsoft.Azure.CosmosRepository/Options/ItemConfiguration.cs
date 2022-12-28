// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Options;

internal class ItemConfiguration
{
    public Type Type { get; }

    public string ContainerName { get; }

    public string PartitionKeyPath { get; }

    public UniqueKeyPolicy? UniqueKeyPolicy { get; }

    public ThroughputProperties? ThroughputProperties { get; }

    public int DefaultTimeToLive { get; }

    public bool SyncContainerProperties { get; }

    public ChangeFeedOptions? ChangeFeedOptions { get; }

    public bool UseStrictTypeChecking { get; }

    public ItemConfiguration(
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
        Type = type;
        ContainerName = containerName;
        PartitionKeyPath = partitionKeyPath;
        UniqueKeyPolicy = uniqueKeyPolicy;
        ThroughputProperties = throughputProperties;
        UseStrictTypeChecking = useStrictTypeChecking;
        DefaultTimeToLive = defaultTimeToLive;
        SyncContainerProperties = syncContainerProperties;
        ChangeFeedOptions = changeFeedOptions;
    }
}