// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Specification;

public class SpecificationPagingSamples
{
    private readonly IRepository<Person> _repository;

    public SpecificationPagingSamples(IRepository<Person> repository)
    {
        _repository = repository;
    }
    public async Task BasicPageAsync()
    {
        double totalCharge = 0;
        OffsetByPageNumberSpecification<Person> specification = new(1, 25);
        IPageQueryResult<Person> page = await _repository.QueryAsync(specification);
        while (page.HasNextPage is not null && page.HasNextPage.Value)
        {
            foreach (Person person in page.Items)
            {
                Console.WriteLine(person);
            }
            totalCharge += page.Charge;
            specification.NextPage();
            page = await _repository.QueryAsync(specification);
            Console.WriteLine($"Get page {page.PageNumber} 25 results cost {page.Charge}");
        }
        Console.WriteLine($"Total Charge {totalCharge} RU's");
    }


    public async Task BasicScrollingAsync()
    {
        double totalCharge = 0;

        ContinuationTokenSpecification<Person> specification = new(null, pageSize: 25);
        IPage<Person> page = await _repository.QueryAsync(specification);
        specification.UpdateContinuationToken(page.Continuation);
        int totalItems = 0;
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
            page = await _repository.QueryAsync(specification);
        }

        Console.WriteLine($"Last 50 results cost {page.Charge}");
        Console.WriteLine($"Total Charge {totalCharge} RU's");
    }
}
