// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Attributes;

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// Exception which is thrown the PersistAsync is used with the aggregate overload and the aggregate doesn't provide the attribute
/// </summary>
/// <remarks>
/// Ceates a new <see cref="EventItemPartitionKeyAttributeRequiredException" />
/// </remarks>
/// <param name="aggregateType">Type of the aggregate which had the invalid configuration. Used to build up the message</param>
public class EventItemPartitionKeyAttributeRequiredException(Type aggregateType) : Exception($"A {nameof(EventItemPartitionKeyAttribute)} must be present on a property in {aggregateType.Name} or you must specify the partition key explicitly")
{
}
