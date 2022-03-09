// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing;

/// <summary>
/// The options that can be used to configure the change feed processor consuming new events.
/// </summary>
/// <typeparam name="TEventItem">Teh type of <see cref="EventItem"/></typeparam>
public class EventSourcingProcessorOptions<TEventItem> where TEventItem : EventItem
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