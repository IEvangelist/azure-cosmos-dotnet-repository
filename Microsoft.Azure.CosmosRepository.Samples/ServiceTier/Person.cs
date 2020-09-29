// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;
using System;

namespace ServiceTier
{
    public class Person : Item
    {
        public DateTime BirthDate { get; set; }

        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;

        [JsonIgnore]
        public int AgeInYears =>
            (int)DateTime.Now.Subtract(BirthDate).TotalDays / 365; // Days in a year

        public override string ToString() =>
            MiddleName is null
                ? $"{FirstName} {LastName} ({AgeInYears} years old, born {BirthDate:MMM dd, yyyy})"
                : $"{FirstName} {MiddleName} {LastName} ({AgeInYears} years old, born {BirthDate:MMM dd, yyyy})";
    }
}
