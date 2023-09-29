// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

class DefaultChangeFeedOptionsProvider(IOptionsMonitor<RepositoryOptions> optionsMonitor) : IChangeFeedOptionsProvider
{
    private readonly RepositoryOptions _repositoryOptions = optionsMonitor.CurrentValue;

    public ChangeFeedOptions GetOptionsForItems(IReadOnlyList<Type> items)
    {
        var changeFeedOptions = _repositoryOptions.ContainerBuilder
            .Options
            .Where(x => items.Contains(x.Type) && x.ChangeFeedOptions is not null)
            .Select(x => x.ChangeFeedOptions!)
            .ToList();

        ChangeFeedOptions? sample = changeFeedOptions.FirstOrDefault();

        return changeFeedOptions.All(x => x.IsTheSameAs(sample)) is false
            ? throw new MissMatchedChangeFeedOptionsException(
                $"The {nameof(ChangeFeedOptions)} for the given types are miss matched", items)
            : sample ?? new(typeof(ChangeFeedOptions));
    }
}