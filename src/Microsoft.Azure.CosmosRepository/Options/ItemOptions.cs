// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.ChangeFeed;

namespace Microsoft.Azure.CosmosRepository.Options
{
    internal class ItemOptions
    {
        public Type Type { get; }

        public string ContainerName { get; }

        public string PartitionKeyPath { get; }

        public UniqueKeyPolicy UniqueKeyPolicy { get; }

        public ThroughputProperties ThroughputProperties { get; }

        public int DefaultTimeToLive { get; }

        public bool SyncContainerProperties { get; }

        public ChangeFeedOptions ChangeFeedOptions { get; } = null;

        public ItemOptions(Type type, string containerName, string partitionKeyPath, UniqueKeyPolicy uniqueKeyPolicy,
            ThroughputProperties throughputProperties, int defaultTimeToLive = -1, bool syncContainerProperties = false,
            ChangeFeedOptions changeFeedOptions = null)
        {
            Type = type;
            ContainerName = containerName;
            PartitionKeyPath = partitionKeyPath;
            UniqueKeyPolicy = uniqueKeyPolicy;
            ThroughputProperties = throughputProperties;
            DefaultTimeToLive = defaultTimeToLive;
            SyncContainerProperties = syncContainerProperties;
            ChangeFeedOptions = changeFeedOptions;
        }
    }
}