// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Options
{
    internal class ItemOptions
    {
        public Type Type { get; }

        public string ContainerName { get; }

        public string PartitionKeyPath { get; }

        public UniqueKeyPolicy UniqueKeyPolicy { get; }

        public ItemOptions(Type type, string containerName, string partitionKeyPath, UniqueKeyPolicy uniqueKeyPolicy)
        {
            Type = type;
            ContainerName = containerName;
            PartitionKeyPath = partitionKeyPath;
            UniqueKeyPolicy = uniqueKeyPolicy;
        }
    }
}