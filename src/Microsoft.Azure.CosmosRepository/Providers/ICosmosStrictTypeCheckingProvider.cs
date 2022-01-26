// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    interface ICosmosStrictTypeCheckingProvider
    {
        bool UseStrictTypeChecking<TItem>()
            where TItem : IItem;

        bool UseStrictTypeChecking(Type itemType);
    }
}