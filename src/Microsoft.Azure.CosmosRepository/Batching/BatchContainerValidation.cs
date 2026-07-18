namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// Shared validation ensuring that every item type in a batch resolves to
/// the same container. When <see cref="RepositoryOptions.ContainerPerItemType"/>
/// is false all item types share the single physical container, so any
/// combination is valid.
/// </summary>
internal static class BatchContainerValidation
{
    internal const string MismatchMessage =
        "The item types provided are not all configured to use the same container";

    internal static void EnsureSameContainer(
        IEnumerable<Type> itemTypes,
        RepositoryOptions options,
        Func<Type, string> getContainerName)
    {
        if (options.ContainerPerItemType is false)
        {
            return;
        }

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
