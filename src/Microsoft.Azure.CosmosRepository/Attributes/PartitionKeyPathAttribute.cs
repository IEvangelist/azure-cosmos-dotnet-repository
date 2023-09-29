// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Attributes;

/// <summary>
/// The partition key path attribute exposes the ability to declaratively
/// specify an <see cref="IItem"/> partition key path. This attribute should be used in
/// conjunction with a <see cref="Newtonsoft.Json.JsonPropertyAttribute"/> on the <see cref="IItem"/> property
/// whose value will act as the partition key. Partition key paths should start with "/",
/// for example "/partition". For more information,
/// see https://docs.microsoft.com/azure/cosmos-db/partitioning-overview.
/// </summary>
/// <remarks>
/// By default, "/id" is used.
/// </remarks>
/// <remarks>
/// Constructor accepting the <paramref name="path"/> of the partition key for a given <see cref="IItem"/>.
/// </remarks>
/// <param name="path"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class PartitionKeyPathAttribute(string path) : Attribute
{
    /// <summary>
    /// Gets the path of the parition key.
    /// </summary>
    public string Path { get; } = path ?? throw new ArgumentNullException(nameof(path), "A path is required.");
}
