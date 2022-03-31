// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.Extensions
{
    internal static class JObjectExtensions
    {
        public static void AddOrUpdateProperty(this JObject jObject, string propertyName, object value)
        {
            JProperty? property = jObject.Property(propertyName);

            if (property is null)
            {
                property = new JProperty(propertyName, value);
                jObject.Add(property);
            }
            else
            {
                property.Value = JToken.FromObject(value);
            }
        }
    }
}