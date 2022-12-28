// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// Thrown when using the _eventStore.ReadAggregateAsync{TAggregateRoot} method and there is not static method named Replay defined.
/// </summary>
public class ReplayMethodNotDefinedException : Exception
{
    /// <summary>
    /// Creates an <see cref="ReplayMethodNotDefinedException"/>
    /// </summary>
    public ReplayMethodNotDefinedException(MemberInfo aggregateType) :
        base($"The {nameof(IAggregateRoot)} of type {aggregateType.Name} does not have a public static TAggregateRoot Replay(List<DomainEvent> events) method defined")
    {

    }
}