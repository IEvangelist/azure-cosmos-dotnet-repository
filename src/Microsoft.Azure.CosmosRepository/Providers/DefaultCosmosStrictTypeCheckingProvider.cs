// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

class DefaultCosmosStrictTypeCheckingProvider(IOptions<RepositoryOptions> options) : ICosmosStrictTypeCheckingProvider
{
    public bool UseStrictTypeChecking<TItem>() where TItem : IItem =>
        UseStrictTypeChecking(typeof(TItem));

    public bool UseStrictTypeChecking(Type itemType)
    {
        ContainerOptionsBuilder? itemOptions = options.Value.GetContainerOptions(itemType);

        return itemOptions?.UseStrictTypeChecking ?? true;
    }
}