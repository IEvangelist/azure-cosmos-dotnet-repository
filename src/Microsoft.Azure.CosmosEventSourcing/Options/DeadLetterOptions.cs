// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Projections;

namespace Microsoft.Azure.CosmosEventSourcing.Options;

/// <summary>
/// Options used to configure where failed projections will be stored.
/// </summary>
public class DeadLetterOptions<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey
{

}