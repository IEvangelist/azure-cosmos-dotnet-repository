// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;

namespace Microsoft.Azure.CosmosRepository.Internals
{
    internal class InternalPatchOperation
    {
        public PropertyInfo PropertyInfo { get; }

        public object NewValue { get; }

        public InternalPatchOperation(PropertyInfo propertyInfo, object newValue)
        {
            PropertyInfo = propertyInfo;
            NewValue = newValue;
        }
    }
}