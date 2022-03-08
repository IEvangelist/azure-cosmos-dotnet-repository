// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public interface IItemMapper<in TItem, TMapped>
    where TItem : IItemWithEtag
    where TMapped : class
{
    ValueTask<TMapped> MapAsync(TItem item);
}