// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Options;

namespace Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;

public class DefaultCosmosItemConfiguration<TItem>(
    IOptionsMonitor<AzureCosmosDbRepositorySettings> optionsMonitor) : ICosmosItemConfiguration<TItem> where TItem : IItem
{
    private readonly AzureCosmosDbRepositorySettings _settings = optionsMonitor.CurrentValue;

    public string Container => _settings.DefaultContainerName;

    public string PartitionKeyPath => _settings.DefaultPartitionKeyPath;
}