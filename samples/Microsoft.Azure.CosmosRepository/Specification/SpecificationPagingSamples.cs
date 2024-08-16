// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Specification;

public class SpecificationPagingSamples(IRepository<Person> repository)
{
    public async Task BasicPageAsync()
    {
        double totalCharge = 0;
        OffsetByPageNumberSpecification<Person> specification = new(1, 25);
        IPageQueryResult<Person> page = await repository.QueryAsync(specification);
        while (page.HasNextPage is not null && page.HasNextPage.Value)
        {
            foreach (Person person in page.Items)
            {
                Console.WriteLine(person);
            }
            totalCharge += page.Charge;
            specification.NextPage();
            page = await repository.QueryAsync(specification);
            Console.WriteLine($"Get page {page.PageNumber} 25 results cost {page.Charge}");
        }
        Console.WriteLine($"Total Charge {totalCharge} RU's");
    }


    public async Task BasicScrollingAsync()
    {
        double totalCharge = 0;

        ContinuationTokenSpecification<Person> specification = new(null, pageSize: 25);
        IPage<Person> page = await repository.QueryAsync(specification);
        specification.UpdateContinuationToken(page.Continuation);
        var totalItems = 0;
        while (totalItems < page.Total)
        {
            foreach (Person person in page.Items)
            {
                Console.WriteLine(person);
            }
            totalItems += page.Items.Count;
            totalCharge += page.Charge;
            Console.WriteLine($"First 25 results cost {page.Charge}");
            specification.UpdateContinuationToken(page.Continuation);
            page = await repository.QueryAsync(specification);
        }

        Console.WriteLine($"Last 50 results cost {page.Charge}");
        Console.WriteLine($"Total Charge {totalCharge} RU's");
    }
}
