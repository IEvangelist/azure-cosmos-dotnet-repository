// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;

namespace Microsoft.Azure.CosmosEventSourcing.Attributes;

/// <summary>
/// An attribute to mark the partition key in a domain aggregate for use in an event item
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class EventItemPartitionKeyAttribute : Attribute
{

}