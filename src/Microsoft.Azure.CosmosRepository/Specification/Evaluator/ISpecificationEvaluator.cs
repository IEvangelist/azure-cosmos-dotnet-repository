// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification.Evaluator
{
    interface ISpecificationEvaluator 
    {
        IQueryable<T> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification, bool evaluateCriteriaOnly = false)
            where T : IItem
            where TResult : IQueryResult<T>;
        TResult GetResult<T, TResult>(IReadOnlyList<T> res, ISpecification<T, TResult> specification, int totalCount, double charge, string continuationToken)
            where T : IItem
            where TResult : IQueryResult<T>;
    }
}
