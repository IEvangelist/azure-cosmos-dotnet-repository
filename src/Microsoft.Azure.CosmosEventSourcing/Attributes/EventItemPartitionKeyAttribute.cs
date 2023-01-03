// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Attributes;

/// <summary>
/// An attribute to mark the partition key in a domain aggregate for use in an event item
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class EventItemPartitionKeyAttribute : Attribute
{

}