// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Options;

public class CosmosEventSourcingOptions
{
    public const string SectionName = "CosmosEventSourcing";

    public bool IsSequenceNumberingDisabled { get; set; }
}