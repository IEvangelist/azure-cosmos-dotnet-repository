// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Attributes;

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// Exception which is thrown the PersistAsync is used with the aggregate overload and the aggregate doesn't provide the attribute
/// </summary>
public class EventItemPartitionKeyAttributeRequiredException : Exception
{
    /// <summary>
    /// Ceates a new <see cref="EventItemPartitionKeyAttributeRequiredException" />
    /// </summary>
    /// <param name="aggregateType">Type of the aggregate which had the invalid configuration. Used to build up the message</param>
    public EventItemPartitionKeyAttributeRequiredException(Type aggregateType) :
        base($"A {nameof(EventItemPartitionKeyAttribute)} must be present on a property in {aggregateType.Name}")
    {

    }
}