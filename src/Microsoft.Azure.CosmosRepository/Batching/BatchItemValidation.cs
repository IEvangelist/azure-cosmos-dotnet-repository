// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// Shared validation ensuring a queued replace still targets the item it was
/// queued for.
/// </summary>
internal static class BatchItemValidation
{
    internal static void EnsureIdUnchanged(string queuedId, string currentId)
    {
        if (!string.Equals(queuedId, currentId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"The item id changed from '{queuedId}' to '{currentId}' after the replace was queued."
            );
        }
    }
}
