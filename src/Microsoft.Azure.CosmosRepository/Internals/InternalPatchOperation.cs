// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Internals;

internal class InternalPatchOperation(IReadOnlyList<PropertyInfo> propertyInfos, object? newValue, PatchOperationType type, int? index = null)
{
    public PatchOperationType Type { get; } = type;
    public IReadOnlyList<PropertyInfo> PropertyInfos { get; } = propertyInfos;
    public int? Index { get; } = index;
    public object? NewValue { get; } = newValue;
}