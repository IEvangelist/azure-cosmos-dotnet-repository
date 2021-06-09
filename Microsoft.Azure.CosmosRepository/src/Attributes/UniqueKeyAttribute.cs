// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    class UniqueKeyAttribute : Attribute
    {
        public string KeyName { get; } = "uniqueKey";

        public UniqueKeyAttribute(string keyName) =>
            KeyName = keyName ?? throw new ArgumentNullException(nameof(keyName), "A keyname is required.");

    }

}
