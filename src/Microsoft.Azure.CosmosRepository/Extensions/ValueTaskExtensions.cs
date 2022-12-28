// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Extensions;

/// <summary>
/// A set of useful extension methods for a <see cref="ValueTask{TResult}"/>
/// </summary>
public static class ValueTaskExtensions
{
    /// <summary>
    /// Converts a <see cref="ValueTask"/> of <see cref="IEnumerable{T}"/> to a <see cref="List{T}"/>
    /// </summary>
    /// <param name="valueTask">The value task</param>
    /// <typeparam name="T">The type of <see cref="IEnumerable{T}"/></typeparam>
    public static async ValueTask<List<T>> ToListAsync<T>(this ValueTask<IEnumerable<T>> valueTask)
    {
        IEnumerable<T> items = await valueTask;
        return items.ToList();
    }

    /// <summary>
    /// Returns the first element in the sequence or the default.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="valueTask">The value task</param>
    /// <returns>Returns the first item in the enumerable or the default.</returns>
    public static async ValueTask<T?> FirstOrDefaultAsync<T>(this ValueTask<IEnumerable<T>> valueTask)
    {
        IEnumerable<T> items = await valueTask;
        return items.FirstOrDefault();
    }

    /// <summary>
    /// Returns the first element in the sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="valueTask">The value task</param>
    /// <exception cref="System.InvalidOperationException">The source sequence is empty.</exception>
    /// <returns>Returns the first element in the sequence.</returns>
    public static async ValueTask<T> FirstAsync<T>(this ValueTask<IEnumerable<T>> valueTask)
    {
        IEnumerable<T> items = await valueTask;
        return items.First();
    }

    /// <summary>
    /// Returns the last element in the sequence.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="valueTask">The value task</param>
    /// <returns>Returns the last element in the sequence.</returns>
    public static async ValueTask<T?> LastOrDefaultAsync<T>(this ValueTask<IEnumerable<T>> valueTask)
    {
        IEnumerable<T> items = await valueTask;
        return items.LastOrDefault();
    }

    /// <summary>
    /// Returns the last element in the sequence.
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <param name="valueTask">The value task</param>
    /// <exception cref="System.InvalidOperationException">The source sequence is empty.</exception>
    /// <returns></returns>
    public static async ValueTask<T> LastAsync<T>(this ValueTask<IEnumerable<T>> valueTask)
    {
        IEnumerable<T> items = await valueTask;
        return items.Last();
    }
}