// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ISpecificationBuilder<T, TResult>
        where T : IItem
        where TResult : IQueryResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        BaseSpecification<T, TResult> Specification { get; }
    }
}
