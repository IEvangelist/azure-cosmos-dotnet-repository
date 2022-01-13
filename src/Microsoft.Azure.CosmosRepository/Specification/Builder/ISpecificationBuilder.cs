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
    public interface ISpecificationBuilder<T>
        where T : IItem
    {
        /// <summary>
        /// 
        /// </summary>
        BaseSpecification<T> Specification { get; }
    }
}
