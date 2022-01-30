// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing;

public interface IPersistedEvent
{
    string EventName { get; }
}