// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Projections;

namespace Microsoft.Azure.CosmosEventSourcing.Options;

/// <summary>
/// The options that can be used to configure the change feed processor consuming new events.
/// </summary>
/// <typeparam name="TEventItem">Teh type of <see cref="EventItem"/></typeparam>
/// <typeparam name="TProjectionKey">The projection used for the processor.</typeparam>
public class EventSourcingProcessorOptions<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey
{
    /// <summary>
    /// The instance name of the processor.
    /// </summary>
    public string InstanceName { get; set; } = "default-processor-instance-name";

    /// <summary>
    /// The poll interval to query the change feed processor
    /// </summary>
    public TimeSpan? PollInterval { get; set; }

    /// <summary>
    /// The processor name provided to the change feed processor library.
    /// </summary>
    public string ProcessorName { get; set; } = "cosmos-repository-pattern-processor";
}