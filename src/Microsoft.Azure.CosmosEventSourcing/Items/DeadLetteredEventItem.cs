// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Microsoft.Azure.CosmosEventSourcing.Models;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcing.Items;

/// <summary>
/// An item implementation that is used when processing an <see cref="DomainEvent"/> has failed.
/// </summary>
public class DeadLetteredEventItem<TEventItem> : Item
{
    /// <summary>
    /// The event item that failed to be processed.
    /// </summary>
    public TEventItem EventItem { get; set; }

    /// <summary>
    /// The <see cref="DateTime"/> in UTC format when the event failed.
    /// </summary>
    public DateTime FailedAtUtc { get; set; }

    /// <summary>
    /// The name of the processor that failed.
    /// </summary>
    public string ProcessorName { get; set; }

    /// <summary>
    /// The name of the instance that the failure occured on.
    /// </summary>
    public string InstanceName { get; set; }

    /// <summary>
    /// The name of the <see cref="IProjectionKey"/> used by the processor.
    /// </summary>
    public string ProjectionKeyName { get; set; }

    /// <summary>
    /// The value used to partition the item by.
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// The details of the exception that caused the failure.
    /// </summary>
    public ExceptionDetails Exception { get; set; }

    /// <inheritdoc />
    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    /// <summary>
    /// Creates an instance of an <see cref="DeadLetteredEventItem{TEventItem}"/>
    /// </summary>
    /// <param name="eventItem"></param>
    /// <param name="processorName"></param>
    /// <param name="instanceName"></param>
    /// <param name="projectionKeyName"></param>
    /// <param name="exception"></param>
    public DeadLetteredEventItem(
        TEventItem eventItem,
        string processorName,
        string instanceName,
        string projectionKeyName,
        ExceptionDetails exception)
    {
        //TODO: finish XML comments.
        EventItem = eventItem;
        ProcessorName = processorName;
        InstanceName = instanceName;
        ProjectionKeyName = projectionKeyName;
        Exception = exception;
        FailedAtUtc = DateTime.UtcNow;
        PartitionKey = Id;
    }
}