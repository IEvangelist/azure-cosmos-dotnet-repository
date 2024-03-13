// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Attributes;

/// <summary>
/// The partition key path attribute exposes the ability to declaratively
/// specify an <see cref="IItem"/> partition key path. This attribute should be used in
/// conjunction with a <see cref="Newtonsoft.Json.JsonPropertyAttribute"/> on the <see cref="IItem"/> property
/// whose value will act as the partition key. Partition key paths should start with "/",
/// for example "/partition". For more information,
/// see https://docs.microsoft.com/azure/cosmos-db/partitioning-overview and
/// https://learn.microsoft.com/en-us/azure/cosmos-db/hierarchical-partition-keys
/// </summary>
/// <remarks>
/// By default, "/id" is used.
/// </remarks>
/// <remarks>
/// Constructor accepting the <paramref name="paths"/> of the partition keys for a given <see cref="IItem"/>.
/// </remarks>
/// <param name="paths"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class PartitionKeyPathAttribute(params string[] paths) : Attribute
{
    /// <summary>
    /// Gets the path values of the parition key.
    /// </summary>
    public string[] Paths { get; } = paths != null && paths.Length >= 1 ? paths : throw new ArgumentNullException(nameof(paths), "At least one path is required.");
}

