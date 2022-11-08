// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

interface IChangeFeedOptionsProvider
{
    ChangeFeedOptions GetOptionsForItems(IReadOnlyList<Type> items);
}