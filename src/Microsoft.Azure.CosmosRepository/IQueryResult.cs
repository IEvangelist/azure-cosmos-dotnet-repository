// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.CosmosRepository
{
    /// <summary>
    /// Represent a full set of data from a cosmos query
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IQueryResult<out TItem> where TItem : IItem
    {
        /// <summary>
        /// The items that are in the current page.
        /// </summary>
        IReadOnlyList<TItem> Items { get; }

        /// <summary>
        /// The amount of RU's the given query cost.
        /// </summary>
        double Charge { get; }
    }
}
