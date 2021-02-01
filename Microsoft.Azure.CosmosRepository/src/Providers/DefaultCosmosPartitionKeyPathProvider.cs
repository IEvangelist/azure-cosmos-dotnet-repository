// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc />
    class DefaultCosmosPartitionKeyPathProvider
        : ICosmosPartitionKeyPathProvider
    {
        static readonly Type _partitionKeyNameAttributeType = typeof(PartitionKeyPathAttribute);
        static readonly ConcurrentDictionary<Type, string> _partionKeyNameMap =
            new ConcurrentDictionary<Type, string>();

        /// <inheritdoc />
        public string GetPartitionKeyPath<TItem>() where TItem : IItem =>
            _partionKeyNameMap.GetOrAdd(typeof(TItem), GetPartitionKeyNameFactory);

        static string GetPartitionKeyNameFactory(Type type)
        {
            PartitionKeyPathAttribute attribute =
                Attribute.GetCustomAttribute(type, _partitionKeyNameAttributeType)
                as PartitionKeyPathAttribute;

            return attribute?.Path ?? "/id";
        }
    }
}
