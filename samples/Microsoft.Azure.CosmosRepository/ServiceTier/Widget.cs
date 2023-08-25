// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace ServiceTier;

public class Widget : Item
{
    public string Name { get; set; } = null!;

    public DateTimeOffset CreatedOrUpdatedOn { get; set; } = DateTimeOffset.UtcNow;

    public override string ToString() => $"{Name} was created or updated on {CreatedOrUpdatedOn}";
}
