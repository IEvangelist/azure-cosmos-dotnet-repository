// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WhereExpressionInfo<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        public WhereExpressionInfo(Expression<Func<T, bool>> filter)
        {
            Filter = filter;
        }
        /// <summary>
        /// 
        /// </summary>
        public Expression<Func<T, bool>> Filter { get; }

    }
}
