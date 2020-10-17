// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace ServiceTier
{
    using System;
    using Microsoft.Azure.CosmosRepository;
    using Microsoft.Azure.CosmosRepository.Attributes;
    using Newtonsoft.Json;

    [PartitionKeyPath("/synthetic")]
    public class Person : Item
    {
        [JsonIgnore]
        public int AgeInYears =>
            (int)DateTime.Now.Subtract(this.BirthDate).TotalDays / 365;

        public DateTime BirthDate { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }

        [JsonProperty("synthetic")]
        public string SyntheticPartitionKey =>
            $"{this.FirstName}-{this.LastName}";

        public override string ToString() =>
            this.MiddleName is null
                ? $"{this.FirstName} {this.LastName} ({this.AgeInYears} years old, born {this.BirthDate:MMM dd, yyyy})"
                : $"{this.FirstName} {this.MiddleName} {this.LastName} ({this.AgeInYears} years old, born {this.BirthDate:MMM dd, yyyy})";

        protected override string GetPartitionKeyValue() => this.SyntheticPartitionKey;
    }
}
