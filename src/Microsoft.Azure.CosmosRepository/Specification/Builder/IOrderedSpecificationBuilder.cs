// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <inheritdoc cref="ISpecificationBuilder{T, TResult}"/>
    public interface IOrderedSpecificationBuilder<T, TResult> : ISpecificationBuilder<T, TResult>
        where T : IItem
        where TResult : IQueryResult<T>
    {
    }
}
