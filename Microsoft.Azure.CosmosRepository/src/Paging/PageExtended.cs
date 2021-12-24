// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

#nullable enable
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.CosmosRepository.Paging
{
    /// <inheritdoc/>
    public class PageExtended<T> : Page<T>, IPageExtended<T> where T : IItem
    {
        /// <summary>
        /// Creates a page.
        /// </summary>
        /// <param name="total"></param>
        /// <param name="size"></param>
        /// <param name="items"></param>
        /// <param name="charge"></param>
        /// <param name="continuation"></param>
        internal PageExtended(
            int total,
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
        internal PageExtended(
            int total,
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
        public int TotalPages => GetTotalPages();

        /// <inheritdoc />
        public int? PageNumber { get; }

        /// <inheritdoc />
        public bool HasPreviousPage => PageNumber > 1;

        /// <inheritdoc />
        public bool HasNextPage => PageNumber < TotalPages;

        /// <inheritdoc />
        public int PreviousPageNumber => GetPreviousPageNumber();

        /// <inheritdoc />
        public int NextPageNumber => GetNextPageNumber();

        private int GetNextPageNumber()
        {
            return HasNextPage && PageNumber.HasValue ? PageNumber.Value + 1 : TotalPages;
        }

        private int GetTotalPages()
        {
            return (int)Math.Abs(Math.Ceiling(Total / (double)Size));
        }

        private int GetPreviousPageNumber()
        {
            return HasPreviousPage && PageNumber.HasValue ? PageNumber.Value - 1 : 1;
        }
    }
}