// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Builders
{
    /// <summary>
    ///
    /// </summary>
    public interface IPatchOperationBuilder<TItem> where TItem : IItem
    {
        /// <summary>
        ///
        /// </summary>
        IReadOnlyList<PatchOperation> PatchOperations { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue value);
    }
}