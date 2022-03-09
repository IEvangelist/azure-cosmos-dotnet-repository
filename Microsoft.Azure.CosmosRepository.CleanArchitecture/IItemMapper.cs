// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public interface IMapper<TItem, TMapped>
    where TItem : IItemWithEtag
{
    ValueTask<TMapped> MapAsync(TItem item);
    ValueTask<TItem> MapAsync(TMapped item);
}