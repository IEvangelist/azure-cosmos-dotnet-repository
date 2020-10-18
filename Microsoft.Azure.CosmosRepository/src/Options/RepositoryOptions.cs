// Copyright © IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Options
{
	/// <summary>
	/// A repository options class, representing 
	/// various Azure Cosmos DB configuration settings.
	/// </summary>
	public class RepositoryOptions
	{
		/// <summary>
		/// Gets or sets the cosmos connection string. Primary or secondary connection strings are valid.
		/// </summary>
		public string CosmosConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the name identifier for the cosmos database.
		/// </summary>
		/// <remarks>
		/// Defaults to "database", unless otherwise specified.
		/// </remarks>
		public string DatabaseId { get; set; } = "database";

		/// <summary>
		/// Gets or sets the name identifier for the cosmos container that corresponds to the <see cref="DatabaseId"/>.
		/// </summary>
		/// <remarks>
		/// Defaults to "container", unless otherwise specified.
		/// </remarks>
		public string ContainerId { get; set; } = "container";

		/// <summary>
		/// Gets or sets whether to optimize bandwidth.
		/// When false, the <see cref="ItemRequestOptions.EnableContentResponseOnWrite"/> is set to false and only
		/// headers and status code in the Cosmos DB response for write item operation like Create, Upsert,
		/// Patch and Replace. This reduces networking and CPU load by not sending the resource back over the
		/// network and serializing it on the client.
		/// </summary>
		/// <remarks>
		/// Defaults to true, see: https://devblogs.microsoft.com/cosmosdb/enable-content-response-on-write
		/// </remarks>
		public bool OptimizeBandwidth { get; set; } = true;

		/// <summary>
		/// Gets or sets whether to create a container per item. When true, a container for type `Foo` will be persisted in
		/// a "Foo" container, and type `Bar` will be persisted in a "Bar" container, and so on. When false, all items share
		/// a container - because it doesn't really matter.
		/// </summary>
		/// <remarks>
		/// Defaults to false, see: https://docs.microsoft.com/azure/cosmos-db/how-to-model-partition-example?WC.m_id=dapine
		/// </remarks>
		public bool ContainerPerItemType { get; set; }


		/// <summary>
		/// Gets or sets whether optimistic batching of service requests occurs. Setting this option might
		/// impact the latency of the operations. Hence this option is recommended for non-latency
		/// sensitive scenarios only.
		/// </summary>
		/// <remarks>
		/// Defaults to false, see: https://devblogs.microsoft.com/cosmosdb/introducing-bulk-support-in-the-net-sdk
		/// </remarks>
		public bool AllowBulkExecution { get; set; }
	}
}
