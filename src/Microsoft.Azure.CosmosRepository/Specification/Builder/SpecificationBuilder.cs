// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Builder
{
    internal class SpecificationBuilder<TItem,TResult> : ISpecificationBuilder<TItem,TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>

    {
        public BaseSpecification<TItem,TResult> Specification { get; }

        public SpecificationBuilder(BaseSpecification<TItem, TResult> specification) =>
            Specification = specification;
    }
}
