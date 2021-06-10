// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    class DefaultCosmosUniqueKeyPolicyProvider : ICosmosUniqueKeyPolicyProvider
    {
        static readonly Type _uniqueKeyAttributeType = typeof(UniqueKeyAttribute);

        static readonly ConcurrentDictionary<Type, UniqueKeyPolicy> _uniqueKeyPolicyMap =
            new ConcurrentDictionary<Type, UniqueKeyPolicy>();

        /// <inheritdoc />
        public UniqueKeyPolicy GetUniqueKeyPolicy<TItem>() where TItem : IItem =>
            _uniqueKeyPolicyMap.GetOrAdd(typeof(TItem), GetPartitionKeyNameFactory);

        static UniqueKeyPolicy GetPartitionKeyNameFactory(Type type)
        {
            Dictionary<string, List<PropertyInfo>> dict = new Dictionary<string, List<PropertyInfo>>();
            IEnumerable<PropertyInfo> uniqueKeyed = type.GetProperties().Where((p) => Attribute.IsDefined(p, _uniqueKeyAttributeType));

            foreach (PropertyInfo property in uniqueKeyed)
            {
                string keyName = property.GetCustomAttribute<UniqueKeyAttribute>().KeyName;
                if (!dict.ContainsKey(keyName))
                {
                    dict.Add(keyName, new List<PropertyInfo>());
                }
                dict[keyName].Add(property);
            }

            UniqueKeyPolicy policy = new UniqueKeyPolicy();
            foreach (KeyValuePair<string, List<PropertyInfo>> kv in dict)
            {
                UniqueKey uniqueKey = new UniqueKey();
                foreach (PropertyInfo property in kv.Value)
                {
                    uniqueKey.Paths.Add("/" + property.Name);
                }
                policy.UniqueKeys.Add(uniqueKey);
            }

            return policy;
        }

    }
}
