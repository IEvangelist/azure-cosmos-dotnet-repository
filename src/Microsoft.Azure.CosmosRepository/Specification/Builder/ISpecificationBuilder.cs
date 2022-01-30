// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification.Builder
{
    /// <summary>
    /// Defines a builder that can build a specification
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ISpecificationBuilder<TItem, TResult>
        where TItem : IItem
        where TResult : IQueryResult<TItem>
    {
        /// <summary>
        /// The specification for the <see cref="IItem"/>
        /// </summary>
        BaseSpecification<TItem, TResult> Specification { get; }
    }
}
