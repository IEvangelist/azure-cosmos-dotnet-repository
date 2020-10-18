// Copyright © IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Properties;

using System;

namespace Microsoft.Azure.CosmosRepository.Attributes
{
	/// <summary>
	/// The partition key path attribute exposes the ability to declaratively
	/// specify an <see cref="Item"/> partition key path. This attribute should be used in
	/// conjunction with a <see cref="Newtonsoft.Json.JsonPropertyAttribute"/> on the <see cref="Item"/> property
	/// whose value will act as the partition key. Partition key paths should start with "/",
	/// for example "/partition". For more information,
	/// see https://docs.microsoft.com/azure/cosmos-db/partitioning-overview.
	/// </summary>
	/// <remarks>
	/// By default, "/id" is used.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class PartitionKeyPathAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the path of the parition key.
		/// </summary>
		public string Path { get; } = Constants.DefaultPartitionKeyPath;

		/// <summary>
		/// Constructor accepting the <paramref name="path"/> of the partition key for a given <see cref="Item"/>.
		/// </summary>
		/// <param name="path"></param>
		public PartitionKeyPathAttribute(string path) =>
			Path = path ?? throw new ArgumentNullException(nameof(path), Resources.APathIsRequired);
	}
}
