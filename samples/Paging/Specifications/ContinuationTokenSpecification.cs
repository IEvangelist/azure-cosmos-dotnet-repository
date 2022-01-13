// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Paging.Specifications
{
    internal class ContinuationTokenSpecification : BaseSpecification<Person>
    {
        public ContinuationTokenSpecification(string continutationToken, int pageSize)
        {
            Query.ContinutationToken(continutationToken);
            Query.PageSize(pageSize);
        }

        internal void UpdateContinutationToken(string continuationToken)
        {
            Query.ContinutationToken(continuationToken);
        }
    }
}
