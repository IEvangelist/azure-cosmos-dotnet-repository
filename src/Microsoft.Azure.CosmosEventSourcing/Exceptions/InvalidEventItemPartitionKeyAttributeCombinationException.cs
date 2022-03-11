// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Attributes;

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// Exception which is thrown the PersistAsync is used with the aggregate overload and the aggregate provides multiple InvalidEventItemPartitionKeyAttributeCombinationException
/// </summary>
public class InvalidEventItemPartitionKeyAttributeCombinationException : Exception
{
    /// <summary>
    /// Ceates a new <see cref="InvalidEventItemPartitionKeyAttributeCombinationException" />
    /// </summary>
    /// <param name="aggregateType">Type of the aggregate which had the invalid configuration. Used to build up the message</param>
    public InvalidEventItemPartitionKeyAttributeCombinationException(Type aggregateType) :
        base($"{nameof(EventItemPartitionKeyAttribute)} can not be present on multiple properties in {aggregateType.Name}")
    {

    }
}