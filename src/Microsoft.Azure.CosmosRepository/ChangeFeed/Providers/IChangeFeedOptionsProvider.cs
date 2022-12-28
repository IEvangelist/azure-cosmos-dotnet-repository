// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

interface IChangeFeedOptionsProvider
{
    ChangeFeedOptions GetOptionsForItems(IReadOnlyList<Type> items);
}