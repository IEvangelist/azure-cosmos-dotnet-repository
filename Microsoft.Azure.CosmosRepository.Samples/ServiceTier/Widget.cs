// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace ServiceTier
{
    using System;
    using Microsoft.Azure.CosmosRepository;

    public class Widget : Item
    {
        public DateTimeOffset CreatedOrUpdatedOn { get; set; } = DateTimeOffset.UtcNow;

        public string Name { get; set; } = null!;

        public override string ToString() => $"{this.Name} was created or updated on {this.CreatedOrUpdatedOn}";
    }
}
