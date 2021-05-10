// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc />
    class DefaultCosmosPartitionKeyPathProvider : ICosmosPartitionKeyPathProvider
    {
        static readonly Type _partitionKeyNameAttributeType = typeof(PartitionKeyPathAttribute);
        static readonly ConcurrentDictionary<Type, string> _partionKeyNameMap =
            new ConcurrentDictionary<Type, string>();

        /// <inheritdoc />
        public string GetPartitionKeyPath<TItem>() where TItem : IItem =>
            _partionKeyNameMap.GetOrAdd(typeof(TItem), GetPartitionKeyNameFactory);

        static string GetPartitionKeyNameFactory(Type type)
        {
            Attribute attribute =
                Attribute.GetCustomAttribute(type, _partitionKeyNameAttributeType);

            return attribute is PartitionKeyPathAttribute partitionKeyPathAttribute
                ? partitionKeyPathAttribute.Path
                : "/id";
        }
    }
}
