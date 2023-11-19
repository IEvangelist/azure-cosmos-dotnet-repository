// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Aspire.Microsoft.Azure.CosmosRepository;

public class AzureCosmosDbRepositorySettings : AzureCosmosDbSettings
{
    public string DefaultContainerName { get; set; } = "container";
    public string DefaultPartitionKeyPath { get; set; } = "/id";
}