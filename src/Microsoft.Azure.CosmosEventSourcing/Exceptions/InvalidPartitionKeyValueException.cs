// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// Thrown when the value of the partition key is null
/// </summary>
public class InvalidPartitionKeyValueException : Exception
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="propertyName">The name of the property that was null</param>
    /// <param name="aggregateType">Type of the aggregate which had the invalid configuration. Used to build up the message</param>
    public InvalidPartitionKeyValueException(string propertyName, Type aggregateType) :
        base(
            $"{propertyName} in {aggregateType.Name} was null. This is not a valid partition key value")
    {

    }
}