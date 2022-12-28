// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#nullable enable

namespace Microsoft.Azure.CosmosRepository.Paging;

/// <inheritdoc/>
public class Page<T> : IPage<T> where T : IItem
{
    /// <summary>
    /// Creates a page.
    /// </summary>
    /// <param name="total"></param>
    /// <param name="size"></param>
    /// <param name="items"></param>
    /// <param name="charge"></param>
    /// <param name="continuation"></param>
    internal Page(int? total, int size, IReadOnlyList<T> items, double charge, string? continuation = null)
    {
        Total = total;
        Size = size;
        Items = items;
        Charge = charge;
        Continuation = continuation;
    }

    /// <inheritdoc />
    public int? Total { get; }

    /// <inheritdoc />
    public int Size { get; }

    /// <inheritdoc />
    public IReadOnlyList<T> Items { get; }

    /// <inheritdoc />
    public double Charge { get; }

    /// <inheritdoc />
    public string? Continuation { get; }
}