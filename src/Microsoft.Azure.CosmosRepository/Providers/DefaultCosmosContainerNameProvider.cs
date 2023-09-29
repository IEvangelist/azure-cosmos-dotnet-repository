// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc cref="Microsoft.Azure.CosmosRepository.Providers.ICosmosContainerNameProvider" />
class DefaultCosmosContainerNameProvider(IOptions<RepositoryOptions> options) : ICosmosContainerNameProvider
{
    private readonly IOptions<RepositoryOptions> _options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public string GetContainerName<TItem>() where TItem : IItem =>
        GetContainerName(typeof(TItem));

    public string GetContainerName(Type itemType)
    {
        Type attributeType = typeof(ContainerAttribute);

        var attribute =
            Attribute.GetCustomAttribute(itemType, attributeType);

        ContainerOptionsBuilder? optionsBuilder = _options.Value.GetContainerOptions(itemType);

        if (optionsBuilder is { } && string.IsNullOrWhiteSpace(optionsBuilder.Name) is false)
        {
            return optionsBuilder.Name!;
        }

        return attribute is ContainerAttribute containerAttribute
            ? containerAttribute.Name
            : itemType.Name;
    }
}
