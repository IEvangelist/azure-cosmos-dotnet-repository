// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc cref="Microsoft.Azure.CosmosRepository.Providers.ICosmosPartitionKeyPathProvider" />
class DefaultCosmosPartitionKeyPathProvider(IOptions<RepositoryOptions> options) :
    ICosmosPartitionKeyPathProvider
{
    private readonly IOptions<RepositoryOptions> _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public IEnumerable<string> GetPartitionKeyPaths<TItem>() where TItem : IItem =>
        GetPartitionKeyPaths(typeof(TItem));

    public IEnumerable<string> GetPartitionKeyPaths(Type itemType)
    {
        Type attributeType = typeof(PartitionKeyPathAttribute);

        ContainerOptionsBuilder? optionsBuilder = _options.Value.GetContainerOptions(itemType);

        if (optionsBuilder is { PartitionKeys: { } } && 
            optionsBuilder.PartitionKeys.Any() && 
            optionsBuilder.PartitionKeys.All(x => !string.IsNullOrWhiteSpace(x)))
        {
           return optionsBuilder.PartitionKeys;
        }

        return Attribute.GetCustomAttribute(
            itemType, attributeType) is PartitionKeyPathAttribute partitionKeyPathAttribute
            ? partitionKeyPathAttribute.Paths
            : ["/id"];
    }

  
}
