// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Microsoft.Azure.Cosmos;

namespace Aspire.Microsoft.Azure.CosmosRepository.Containers;

public interface IItemContainerProvider
{
    Task<Container> GetContainerAsync<TItem>(
        CancellationToken cancellationToken = default) where TItem : IItem;
}