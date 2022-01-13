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
    internal class PageNumberPageSizeSpecification : BaseSpecification<Person>
    {
        public PageNumberPageSizeSpecification(int pageNumber, int pageSize)
        {
            Query.PageSize(pageSize);
            Query.PageNumber(pageNumber);   
        }

        internal void NextPage()
        {
            Query.PageNumber(PageNumber.Value + 1);
        }
    }
}
