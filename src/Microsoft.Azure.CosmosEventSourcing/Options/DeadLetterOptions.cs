// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Options;

/// <summary>
/// Options used to configure where failed projections will be stored.
/// </summary>
public class DeadLetterOptions
{
    /// <summary>
    /// The name of the container to store dead-lettered events.
    /// </summary>
    public string ContainerName { get; set; } = "dead-lettered-events";

}