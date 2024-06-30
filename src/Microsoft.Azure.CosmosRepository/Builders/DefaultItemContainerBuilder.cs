// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

/// <inheritdoc/>
internal class DefaultItemContainerBuilder : IItemContainerBuilder
{
    private readonly List<ContainerOptionsBuilder> _options = [];

    public IReadOnlyList<ContainerOptionsBuilder> Options => _options;

    public IItemContainerBuilder Configure<TItem>(Action<ContainerOptionsBuilder> containerOptions) where TItem : IItem
    {
        if (containerOptions is null) throw new ArgumentNullException(nameof(containerOptions));

        ContainerOptionsBuilder optionsBuilder = new(typeof(TItem));

        containerOptions(optionsBuilder);

        _options.Add(optionsBuilder);

        return this;
    }
}