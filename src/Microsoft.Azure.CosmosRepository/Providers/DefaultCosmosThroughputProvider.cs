// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc/>
class DefaultCosmosThroughputProvider : ICosmosThroughputProvider
{
    readonly IOptions<RepositoryOptions> _options;

    public DefaultCosmosThroughputProvider(IOptions<RepositoryOptions> options) =>
        _options = options;

    /// <inheritdoc/>
    public ThroughputProperties? GetThroughputProperties<TItem>() where TItem : IItem =>
        GetThroughputProperties(typeof(TItem));

    public ThroughputProperties? GetThroughputProperties(Type itemType)
    {
        ContainerOptionsBuilder? currentItemsOptions = _options.Value.GetContainerOptions(itemType);

        if (currentItemsOptions is null)
        {
            return ThroughputProperties.CreateManualThroughput(400);
        }

        foreach (ContainerOptionsBuilder option in _options.Value.GetContainerSharedContainerOptions(itemType))
        {
            if (option is { ThroughputProperties: null })
            {
                return null;
            }

            if (currentItemsOptions.ThroughputProperties?.Throughput != null &&
                option.ThroughputProperties.Throughput != currentItemsOptions.ThroughputProperties.Throughput)
            {
                throw new InvalidOperationException($"The container {option.Name} has conflicting manual throughput properties. " +
                                                    $"({option.Type.Name}->{option.ThroughputProperties.Throughput} vs " +
                                                    $"{currentItemsOptions.Type.Name}->{currentItemsOptions.ThroughputProperties.Throughput}).");
            }

            if (option.ThroughputProperties.AutoscaleMaxThroughput != null &&
                option.ThroughputProperties.AutoscaleMaxThroughput != currentItemsOptions.ThroughputProperties?.AutoscaleMaxThroughput)
            {
                throw new InvalidOperationException($"The container {option.Name} has conflicting autoscale throughput properties. " +
                                                    $"({option.Type.Name}->{option.ThroughputProperties.AutoscaleMaxThroughput} vs " +
                                                    $"{currentItemsOptions.Type.Name}->{currentItemsOptions.ThroughputProperties?.AutoscaleMaxThroughput}).");
            }
        }

        return currentItemsOptions.ThroughputProperties;
    }
}