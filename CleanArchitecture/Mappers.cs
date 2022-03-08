// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.CleanArchitecture;

namespace CleanArchitecture;

public class PersonItemMapper : IItemMapper<PersonItem, PersonEntity>
{
    public ValueTask<PersonEntity> MapAsync(PersonItem item) => ValueTask.FromResult(item.ToEntity());
}

public class PersonEntityMapper : IMappedItemMapper<PersonEntity, PersonItem>
{
    public ValueTask<PersonItem> MapAsync(PersonEntity entity) => ValueTask.FromResult(entity.ToItem());
}