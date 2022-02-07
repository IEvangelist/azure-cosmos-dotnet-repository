// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing;

public class EventSourcingProcessorOptions<TSourceEvent> where TSourceEvent : EventSource
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