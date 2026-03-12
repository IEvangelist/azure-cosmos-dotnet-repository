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
        ContainerOptionsBuilder? optionsBuilder = _options.Value.GetContainerOptions(itemType);
        string partitionKeyPath = GetPartitionKeyPath(itemType, optionsBuilder);

        if (optionsBuilder is not null)
        {
            foreach (ContainerOptionsBuilder sharedContainerOptions in _options.Value.GetContainerSharedContainerOptions(itemType))
            {
                string sharedPartitionKeyPath = GetPartitionKeyPath(sharedContainerOptions.Type, sharedContainerOptions);

                if (string.Equals(sharedPartitionKeyPath, partitionKeyPath, StringComparison.Ordinal) is false)
                {
                    string containerName = optionsBuilder.Name ?? _options.Value.ContainerId;

                    throw new InvalidOperationException(
                        $"The container {containerName} has conflicting partition key paths. " +
                        $"({optionsBuilder.Type.Name}->{partitionKeyPath} vs " +
                        $"{sharedContainerOptions.Type.Name}->{sharedPartitionKeyPath}).");
                }
            }
        }

        return partitionKeyPath;
    }

    private static string GetPartitionKeyPath(Type itemType, ContainerOptionsBuilder? optionsBuilder)
    {
        Type attributeType = typeof(PartitionKeyPathAttribute);

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
