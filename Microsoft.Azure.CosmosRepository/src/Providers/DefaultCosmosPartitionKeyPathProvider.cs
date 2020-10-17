// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers
{
    using System;
    using System.Collections.Concurrent;
    using Microsoft.Azure.CosmosRepository.Attributes;

    /// <inheritdoc />
    internal class DefaultCosmosPartitionKeyPathProvider :
        ICosmosPartitionKeyPathProvider
    {
        private static readonly ConcurrentDictionary<Type, string> partionKeyNameMap =
            new ConcurrentDictionary<Type, string>();

        private static readonly Type partitionKeyNameAttributeType = typeof(PartitionKeyPathAttribute);

        /// <inheritdoc />
        public string GetPartitionKeyPath<TItem>() where TItem : Item =>
            partionKeyNameMap.GetOrAdd(typeof(TItem), GetPartitionKeyNameFactory);

        private static string GetPartitionKeyNameFactory(Type type)
        {
            PartitionKeyPathAttribute? attribute =
                Attribute.GetCustomAttribute(type, partitionKeyNameAttributeType)
                as PartitionKeyPathAttribute;

            return attribute?.Path ?? "/id";
        }
    }
}
