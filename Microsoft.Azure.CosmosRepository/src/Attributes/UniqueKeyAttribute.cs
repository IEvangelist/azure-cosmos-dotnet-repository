// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.Attributes
{
    /// <summary>
    /// The unique key attribute exposes the ability to declaratively
    /// specify an <see cref="IItem"/>'s properties that can contribute to a unique key constraint.
    /// For more information, see https://docs.microsoft.com/azure/cosmos-db/unique-keys.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class UniqueKeyAttribute : Attribute
    {
        /// <summary>
        /// Gets the key name that represents the unique key.
        /// </summary>
        public string KeyName { get; } = "uniqueKey";

        /// <summary>
        /// Constructor accepting the <paramref name="keyName"/> for a given <see cref="IItem"/>'s property.
        /// </summary>
        /// <param name="keyName"></param>
        public UniqueKeyAttribute(string keyName) =>
            KeyName = keyName ?? throw new ArgumentNullException(nameof(keyName), "A keyname is required.");
    }
}
