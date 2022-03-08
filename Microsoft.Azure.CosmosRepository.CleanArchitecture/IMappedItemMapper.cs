// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public interface IMappedItemMapper<in TMapped, TItem>
    where TMapped : class
    where TItem : IItemWithEtag
{
    ValueTask<TItem> MapAsync(TMapped item);
}