// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Internals;

internal class InternalPatchOperation(IReadOnlyList<PropertyInfo> propertyInfos, object? newValue, PatchOperationType type)
{
    public PatchOperationType Type { get; } = type;

    public IReadOnlyList<PropertyInfo> PropertyInfos { get; } = propertyInfos;

    public object? NewValue { get; } = newValue;
}