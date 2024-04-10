// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Options;

/// <summary>
/// A repository options class, representing
/// various Azure Cosmos DB configuration settings.
/// </summary>
public class RepositoryOptions
{
    /// <summary>
    /// The configuration key used for the connection string.
    /// </summary>
    public const string CosmosConnectionStringConfigKey = "RepositoryOptions:CosmosConnectionString";

    /// <summary>
    /// The configuration key used for the database ID.
    /// </summary>
    public const string DatabaseIdConfigKey = "RepositoryOptions:DatabaseId";

    /// <summary>
    /// Gets or sets the cosmos connection string. Primary or secondary connection strings are valid.
    /// </summary>
    public virtual string? CosmosConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the cosmos account endpoint URI. This can be retrieved from the Overview section of the Azure Portal.
    /// This is required if you are authenticating using tokens.
    /// <remarks>
    /// In the form of https://{databaseaccount}.documents.azure.com:443/, see: https://docs.microsoft.com/en-us/rest/api/cosmos-db/cosmosdb-resource-uri-syntax-for-rest
    /// </remarks>
    /// </summary>
    public string? AccountEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the name identifier for the cosmos database.
    /// </summary>
    /// <remarks>
    /// Defaults to "database", unless otherwise specified.
    /// </remarks>
    public virtual string DatabaseId { get; set; } = "database";

    /// <summary>
    /// Gets or sets the name identifier for the cosmos container that corresponds to the <see cref="DatabaseId"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to "container", unless otherwise specified.
    /// </remarks>
    public virtual string ContainerId { get; set; } = "container";

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
    ///Defaults to false, see: https://docs.microsoft.com/azure/cosmos-db/how-to-model-partition-example?WC.m_id=dapine
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

    /// <summary>
    /// Get or sets whether or not to sync all container properties. Setting this option will mean all containers when created for the first time will ensure that
    /// the container properties are up to date.
    /// <remarks>If you want to specify this at the container level see <see cref="ContainerBuilder"/></remarks>
    /// </summary>
    public bool SyncAllContainerProperties { get; set; }

    /// <summary>
    /// Gets or sets the repository serialization options.
    /// </summary>
    public RepositorySerializationOptions? SerializationOptions { get; set; }

    /// <summary>
    /// The <see cref="TokenCredential"/> which can be used to access azure resources, including Cosmos DB.
    /// </summary>
    public TokenCredential? TokenCredential { get; set; } = null;

    /// <summary>
    /// A builder to configure containers.
    /// Ensure that ContainerPerItemType is set to true for the container name configured here to take affect.
    /// </summary>
    public IItemContainerBuilder ContainerBuilder { get; } = new DefaultItemContainerBuilder();

    /// <summary>
    /// Used to tell the SDK whether or not to try and creates databases and containers if they do not exist.
    /// </summary>
    /// <remarks>This feature is very powerful for local development. However, in scenarios where infrastructure as code is used this may not be required.</remarks>
    public bool IsAutoResourceCreationIfNotExistsEnabled { get; set; } = true;

    /// <summary>
    /// Container options provided by the <see cref="Builders.IItemContainerBuilder"/>
    /// </summary>
    internal IReadOnlyList<ContainerOptionsBuilder> ContainerOptions => ContainerBuilder.Options;

    /// <summary>
    /// Get the <see cref="ContainerOptionsBuilder"/> for a given <see cref="IItem"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <returns>null or <see cref="ContainerOptionsBuilder"/>.</returns>
    internal ContainerOptionsBuilder? GetContainerOptions<TItem>() where TItem : IItem =>
        GetContainerOptions(typeof(TItem));

    internal ContainerOptionsBuilder? GetContainerOptions(Type itemType) =>
        ContainerOptions.FirstOrDefault(co => co.Type == itemType);

    /// <summary>
    /// Gets container options for <see cref="IItem"/>s that share the same container.
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="IItem"/> to find common types for.</typeparam>
    /// <returns>A collection of <see cref="ContainerOptionsBuilder"/>s that share the same container.</returns>
    internal IEnumerable<ContainerOptionsBuilder> GetContainerSharedContainerOptions<TItem>() where TItem : IItem
    {
        ContainerOptionsBuilder? containerOptionsBuilder = GetContainerOptions<TItem>();
        return containerOptionsBuilder is not null ? ContainerOptions.Where(co => co.Name == containerOptionsBuilder.Name) : new List<ContainerOptionsBuilder>();
    }

    internal IEnumerable<ContainerOptionsBuilder> GetContainerSharedContainerOptions(Type itemType)
    {
        ContainerOptionsBuilder? containerOptionsBuilder = GetContainerOptions(itemType);
        return containerOptionsBuilder is not null ? ContainerOptions.Where(co => co.Name == containerOptionsBuilder.Name) : new List<ContainerOptionsBuilder>();
    }
}