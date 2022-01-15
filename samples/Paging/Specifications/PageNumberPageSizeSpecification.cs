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
    internal class PageNumberPageSizeSpecificationImplementation : PageNumberPageSizeSpecification<Person>
    {
        public PageNumberPageSizeSpecificationImplementation(int pageNumber, int pageSize):
            base(pageNumber, pageSize)
        {

        }
    }
}
