// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

#nullable enable

namespace Microsoft.Azure.CosmosRepository.Paging;

/// <inheritdoc/>
public class PageQueryResult<T> : Page<T>, IPageQueryResult<T> where T : IItem
{
    /// <summary>
    /// Creates a page.
    /// </summary>
    /// <param name="total">The total.</param>
    /// <param name="size">The size.</param>
    /// <param name="items">The items.</param>
    /// <param name="charge">The charge.</param>
    /// <param name="continuation">The continuation.</param>
    internal PageQueryResult(
        int? total,
        int size,
        IReadOnlyList<T> items,
        double charge,
        string? continuation = null)
        : this(total, null, size, items, charge, continuation)
    { }

    /// <summary>
    /// Creates a page.
    /// </summary>
    /// <param name="total">The total.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="size">The size.</param>
    /// <param name="items">The items.</param>
    /// <param name="charge">The charge.</param>
    /// <param name="continuation">The continuation.</param>
    internal PageQueryResult(
        int? total,
        int? pageNumber,
        int size,
        IReadOnlyList<T> items,
        double charge,
        string? continuation = null)
        : base(total, size, items, charge, continuation)
    {
        PageNumber = pageNumber;
    }

    /// <inheritdoc />
    public int? TotalPages => GetTotalPages();

    /// <inheritdoc />
    public int? PageNumber { get; }

    /// <inheritdoc />
    public bool HasPreviousPage => PageNumber > 1;

    /// <inheritdoc />
    public bool? HasNextPage => TotalPages is not null ? PageNumber < TotalPages : null;

    /// <inheritdoc />
    public int PreviousPageNumber => GetPreviousPageNumber();

    /// <inheritdoc />
    public int? NextPageNumber => GetNextPageNumber();

    private int? GetNextPageNumber()
    {
        if (HasNextPage is not null)
        {
            return HasNextPage.HasValue && PageNumber.HasValue ? PageNumber.Value + 1 : TotalPages ?? null;
        }

        return null;
    }

    private int? GetTotalPages()
    {
        if (Total is not null)
        {
            return (int)Math.Abs(Math.Ceiling(Total.Value / (double)Size));
        }

        return null;
    }

    private int GetPreviousPageNumber() => HasPreviousPage && PageNumber.HasValue ? PageNumber.Value - 1 : 1;
}