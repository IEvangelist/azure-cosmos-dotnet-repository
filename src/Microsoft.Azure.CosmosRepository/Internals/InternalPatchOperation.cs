// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Internals;

internal class InternalPatchOperation
{
    public PatchOperationType Type { get; }
    public PropertyInfo PropertyInfo { get; }

    public object? NewValue { get; }

    public InternalPatchOperation(PropertyInfo propertyInfo, object? newValue, PatchOperationType type)
    {
        PropertyInfo = propertyInfo;
        NewValue = newValue;
        Type = type;
    }
}