// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Items;

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