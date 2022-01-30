// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Builder
{
    /// <inheritdoc cref="IOrderedSpecificationBuilder{TItem, TResult}"/>
    public class OrderedSpecificationBuilder<TItem, TResult> : IOrderedSpecificationBuilder<TItem, TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        /// <inheritdoc/>
        public BaseSpecification<TItem, TResult> Specification { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="specification"></param>
        public OrderedSpecificationBuilder(BaseSpecification<TItem, TResult> specification) =>
            Specification = specification;
    }
}