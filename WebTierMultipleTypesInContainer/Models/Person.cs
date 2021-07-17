// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

namespace WebApplication2
{
    public class Person : Item
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
