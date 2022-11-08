// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.CosmosRepository.Exceptions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

class DefaultChangeFeedOptionsProvider : IChangeFeedOptionsProvider
{
    private readonly RepositoryOptions _repositoryOptions;

    public DefaultChangeFeedOptionsProvider(IOptionsMonitor<RepositoryOptions> optionsMonitor) =>
        _repositoryOptions = optionsMonitor.CurrentValue;

    public ChangeFeedOptions GetOptionsForItems(IReadOnlyList<Type> items)
    {
        List<ChangeFeedOptions> changeFeedOptions = _repositoryOptions.ContainerBuilder
            .Options
            .Where(x => items.Contains(x.Type) && x.ChangeFeedOptions is not null)
            .Select(x => x.ChangeFeedOptions!)
            .ToList();

        ChangeFeedOptions sample = changeFeedOptions.First();

        if (changeFeedOptions.All(x => x.IsTheSameAs(sample)) is false)
        {
            throw new MissMatchedChangeFeedOptionsException(
                $"The {nameof(ChangeFeedOptions)} for the given types are miss matched", items);
        }

        return sample;
    }
}