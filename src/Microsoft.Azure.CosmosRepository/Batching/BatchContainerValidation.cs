// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// Shared validation ensuring that every item type in a batch resolves to
/// the same physical container.
/// </summary>
internal static class BatchContainerValidation
{
    internal const string MismatchMessage =
        "The item types provided are not all configured to use the same container";

    internal static void EnsureSameContainer(
        IEnumerable<Type> itemTypes,
        Func<Type, string> getContainerName)
    {
        string? containerName = null;

        foreach (Type itemType in itemTypes)
        {
            var name = getContainerName(itemType);
            containerName ??= name;

            if (name != containerName)
            {
                throw new InvalidOperationException(MismatchMessage);
            }
        }
    }
}
