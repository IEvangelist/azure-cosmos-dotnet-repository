// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Newtonsoft.Json;

namespace AspireSample.ApiService.Infrastructure;

public class EmployeeItem : Item
{
    public EmployeeItem(string firstName,
        string lastName,
        string storeNumber,
        DateTimeOffset joinedAtUtc)
    {
        FirstName = firstName;
        LastName = lastName;
        StoreNumber = storeNumber;
        JoinedAtUtc = joinedAtUtc;
    }

    public EmployeeItem()
    {

    }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    [JsonPropertyName("storeNumber")]
    [JsonProperty("storeNumber")]
    public string StoreNumber { get; set; } = null!;

    public DateTimeOffset JoinedAtUtc { get; set; }
}