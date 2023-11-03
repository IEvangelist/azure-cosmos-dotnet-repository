// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Options;


/// <summary>
/// Options to control the functionality of the event sourcing library.
/// </summary>
public class CosmosEventSourcingOptions
{
    public const string SectionName = "CosmosEventSourcing";

    /// <summary>
    /// Controls whether or not the aggregate maintains sequencing of events using the sequence number.
    /// </summary>
    public bool IsSequenceNumberingDisabled { get; set; }
}