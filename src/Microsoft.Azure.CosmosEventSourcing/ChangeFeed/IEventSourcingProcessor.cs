// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.ChangeFeed;

namespace Microsoft.Azure.CosmosEventSourcing.ChangeFeed;

/// <summary>
/// A processor that allows the library to consume events from the change feed.
/// </summary>
public interface IEventSourcingProcessor : IContainerChangeFeedProcessor
{

}