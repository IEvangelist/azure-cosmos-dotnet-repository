// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Options;

internal class ItemConfiguration
{
    // Existing constructor
    public ItemConfiguration(
        Type type,
        string containerName,
        string[] partitionKeyPaths,
        UniqueKeyPolicy? uniqueKeyPolicy = null,
        ThroughputProperties? throughputProperties = null,
        int defaultTimeToLive = -1,
        bool syncContainerProperties = false,
        ChangeFeedOptions? changeFeedOptions = null,
        bool useStrictTypeChecking = true)
    {
        Type = type;
        ContainerName = containerName;
        PartitionKeyPaths = partitionKeyPaths;
        UniqueKeyPolicy = uniqueKeyPolicy;
        ThroughputProperties = throughputProperties;
        DefaultTimeToLive = defaultTimeToLive;
        SyncContainerProperties = syncContainerProperties;
        ChangeFeedOptions = changeFeedOptions;
        UseStrictTypeChecking = useStrictTypeChecking;
        
    }

    public Type Type { get; }

    public string ContainerName { get; }

    public string[] PartitionKeyPaths { get; }

    public UniqueKeyPolicy? UniqueKeyPolicy { get; }

    public ThroughputProperties? ThroughputProperties { get; }

    public int DefaultTimeToLive { get; }

    public bool SyncContainerProperties { get; }

    public ChangeFeedOptions? ChangeFeedOptions { get; }

    public bool UseStrictTypeChecking { get; }
}
