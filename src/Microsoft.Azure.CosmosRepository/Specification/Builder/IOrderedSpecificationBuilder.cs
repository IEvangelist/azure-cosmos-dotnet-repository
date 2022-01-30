// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Builder
{
    /// <inheritdoc cref="ISpecificationBuilder{T, TResult}"/>
    public interface IOrderedSpecificationBuilder<TItem, TResult> : ISpecificationBuilder<TItem, TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
    }
}
