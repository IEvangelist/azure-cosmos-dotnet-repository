// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace WebTierMultipleTypesInContainer.Models
{
    public class Animal : Item
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
