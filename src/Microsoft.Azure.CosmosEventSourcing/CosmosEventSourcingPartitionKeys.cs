// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing;

/// <summary>
/// Contains some constants used as partitionKeys
/// </summary>
public static class CosmosEventSourcingPartitionKeys
{
    /// <summary>
    /// The default value used for partitioning.
    /// </summary>
    public const string Default = "/partitionKey";
}