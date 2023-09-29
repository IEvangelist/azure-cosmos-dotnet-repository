// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc cref="Microsoft.Azure.CosmosRepository.Providers.ICosmosPartitionKeyPathProvider" />
class DefaultCosmosPartitionKeyPathProvider(IOptions<RepositoryOptions> options) :
    ICosmosPartitionKeyPathProvider
{
    private readonly IOptions<RepositoryOptions> _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public string GetPartitionKeyPath<TItem>() where TItem : IItem =>
        GetPartitionKeyPath(typeof(TItem));

    public string GetPartitionKeyPath(Type itemType)
    {
        Type attributeType = typeof(PartitionKeyPathAttribute);

        ContainerOptionsBuilder? optionsBuilder = _options.Value.GetContainerOptions(itemType);

        if (optionsBuilder is { } && string.IsNullOrWhiteSpace(optionsBuilder.PartitionKey) is false)
        {
            return optionsBuilder.PartitionKey!;
        }

        return Attribute.GetCustomAttribute(
            itemType, attributeType) is PartitionKeyPathAttribute partitionKeyPathAttribute
            ? partitionKeyPathAttribute.Path
            : "/id";
    }
}
