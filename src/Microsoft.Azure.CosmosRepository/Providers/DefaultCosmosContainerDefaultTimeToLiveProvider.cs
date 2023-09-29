// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc/>
/// <summary>
/// Creates an instance of the <see cref="DefaultCosmosContainerDefaultTimeToLiveProvider"/>.
/// </summary>
/// <param name="options">The repository options.</param>
class DefaultCosmosContainerDefaultTimeToLiveProvider(IOptions<RepositoryOptions> options) : ICosmosContainerDefaultTimeToLiveProvider
{
    private readonly IOptions<RepositoryOptions> _options = options;

    /// <inheritdoc/>
    public int GetDefaultTimeToLive<TItem>() where TItem : IItem =>
        GetDefaultTimeToLive(typeof(TItem));

    public int GetDefaultTimeToLive(Type itemType)
    {
        ContainerOptionsBuilder? options = _options.Value.GetContainerOptions(itemType);

        if (options is null)
        {
            return -1;
        }

        foreach (ContainerOptionsBuilder containerOptions in _options.Value.GetContainerSharedContainerOptions(itemType))
        {
            if (containerOptions.ContainerDefaultTimeToLive != null && containerOptions.ContainerDefaultTimeToLive != options.ContainerDefaultTimeToLive)
            {
                throw new InvalidOperationException($"The container {options.Name} has conflicting default time to live values. " +
                                                    $"({options.Type.Name}->{options.ContainerDefaultTimeToLive} vs " +
                                                    $"{containerOptions.Type.Name}->{containerOptions.ContainerDefaultTimeToLive}).");
            }
        }

        return (int)(options.ContainerDefaultTimeToLive?.TotalSeconds ?? -1);
    }
}