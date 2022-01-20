// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc />
    class DefaultCosmosUniqueKeyPolicyProvider : ICosmosUniqueKeyPolicyProvider
    {
        /// <inheritdoc />
        public UniqueKeyPolicy GetUniqueKeyPolicy<TItem>() where TItem : IItem =>
            GetUniqueKeyPolicy(typeof(TItem));

        public UniqueKeyPolicy GetUniqueKeyPolicy(Type itemType)
        {
            Type attributeType = typeof(UniqueKeyAttribute);

            Dictionary<string, List<string>> keyNameToPathsMap = new();

            foreach ((UniqueKeyAttribute uniqueKey, string propertyName) in itemType.GetProperties()
                         .Where(x => Attribute.IsDefined(x, attributeType))
                         .Select(x => (x.GetCustomAttribute<UniqueKeyAttribute>(), x.Name)))
            {
                string propertyValue = uniqueKey.PropertyPath ?? $"/{propertyName}";

                if (keyNameToPathsMap.ContainsKey(uniqueKey.KeyName))
                {
                    keyNameToPathsMap[uniqueKey.KeyName].Add(propertyValue);
                    continue;
                }

                keyNameToPathsMap[uniqueKey.KeyName] = new List<string> {propertyValue};
            }

            if (!keyNameToPathsMap.Any())
            {
                return null;
            }

            UniqueKeyPolicy policy = new();

            foreach (KeyValuePair<string, List<string>> keyNameToPaths in keyNameToPathsMap)
            {
                UniqueKey key = new();

                foreach (string path in keyNameToPaths.Value)
                {
                    key.Paths.Add(path);
                }

                policy.UniqueKeys.Add(key);
            }

            return policy;
        }
    }
}