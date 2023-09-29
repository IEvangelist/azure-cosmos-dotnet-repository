// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Attributes;

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// Exception which is thrown the PersistAsync is used with the aggregate overload and the aggregate provides multiple InvalidEventItemPartitionKeyAttributeCombinationException
/// </summary>
/// <remarks>
/// Ceates a new <see cref="InvalidEventItemPartitionKeyAttributeCombinationException" />
/// </remarks>
/// <param name="aggregateType">Type of the aggregate which had the invalid configuration. Used to build up the message</param>
public class InvalidEventItemPartitionKeyAttributeCombinationException(Type aggregateType) : Exception($"{nameof(EventItemPartitionKeyAttribute)} can not be present on multiple properties in {aggregateType.Name}")
{
}