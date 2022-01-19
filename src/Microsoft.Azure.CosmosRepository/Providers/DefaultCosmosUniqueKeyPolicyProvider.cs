﻿// Copyright (c) IEvangelist. All rights reserved.
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

            Dictionary<string, HashSet<PropertyInfo>> keyNamesToPropertyMap = new();
            foreach ((PropertyInfo property, string keyName) in
                     itemType.GetProperties()
                         .Where(p => Attribute.IsDefined(p, attributeType))
                         .Select(p => (Property: p, p.GetCustomAttribute<UniqueKeyAttribute>().KeyName)))
            {
                if (!keyNamesToPropertyMap.ContainsKey(keyName))
                {
                    keyNamesToPropertyMap.Add(keyName, new HashSet<PropertyInfo>());
                }
                keyNamesToPropertyMap[keyName].Add(property);
            }

            if (keyNamesToPropertyMap.Count == 0)
            {
                return null;
            }

            UniqueKeyPolicy policy = new();
            foreach ((string keyName, HashSet<PropertyInfo> properties) in
                     keyNamesToPropertyMap.Select(kvp => (kvp.Key, kvp.Value)))
            {
                UniqueKey uniqueKey = new();
                foreach (PropertyInfo property in properties)
                {
                    uniqueKey.Paths.Add($"/{keyName}");
                }
                policy.UniqueKeys.Add(uniqueKey);
            }

            return policy;
        }
    }
}
