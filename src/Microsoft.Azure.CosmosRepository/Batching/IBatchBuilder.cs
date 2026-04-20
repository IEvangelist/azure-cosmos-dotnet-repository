namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// Builds a transactional batch for a single partition key.
/// </summary>
public interface IBatchBuilder
{
    /// <summary>
    /// Adds a create operation for the given item.
    /// </summary>
    /// <typeparam name="TItem">The item type to create.</typeparam>
    /// <param name="item">The item to create.</param>
    /// <returns>The current batch builder.</returns>
    IBatchBuilder CreateItem<TItem>(TItem item) where TItem : IItem;

    /// <summary>
    /// Adds a replace operation for the given item.
    /// </summary>
    /// <typeparam name="TItem">The item type to replace.</typeparam>
    /// <param name="item">The item to replace.</param>
    /// <returns>The current batch builder.</returns>
    IBatchBuilder ReplaceItem<TItem>(TItem item) where TItem : IItem;

    /// <summary>
    /// Adds an upsert operation for the given item.
    /// </summary>
    /// <typeparam name="TItem">The item type to upsert.</typeparam>
    /// <param name="item">The item to upsert.</param>
    /// <returns>The current batch builder.</returns>
    IBatchBuilder UpsertItem<TItem>(TItem item) where TItem : IItem;

    /// <summary>
    /// Adds a delete operation for the given item.
    /// </summary>
    /// <typeparam name="TItem">The item type to delete.</typeparam>
    /// <param name="item">The item to delete.</param>
    /// <returns>The current batch builder.</returns>
    IBatchBuilder DeleteItem<TItem>(TItem item) where TItem : IItem;

    /// <summary>
    /// Adds a delete operation for the given item identifier.
    /// </summary>
    /// <typeparam name="TItem">The item type to delete.</typeparam>
    /// <param name="id">The identifier of the item to delete.</param>
    /// <returns>The current batch builder.</returns>
    IBatchBuilder DeleteItem<TItem>(string id) where TItem : IItem;

    /// <summary>
    /// Executes the batch.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the async batch operation.</returns>
    ValueTask ExecuteAsync(CancellationToken cancellationToken = default);
}
