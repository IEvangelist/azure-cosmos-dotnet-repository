// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Specification;

public class FullSpecificationSamples(IRepository<Person> repository)
{
    private readonly IRepository<Person> _repository = repository;

    public async Task FullContinuationTokenSpecificationAsync(int age)
    {
        UsersOrderByAgeSpecification specification = new(age);
        double totalCharge = 0;
        string continuationToken = null;
        do
        {
            specification.UpdateContinuationToken(continuationToken);

            IPage<Person> page = await _repository.QueryAsync(specification);

            foreach (Person person in page.Items)
            {
                Console.WriteLine(person);
            }

            totalCharge += page.Charge;
            continuationToken = page.Continuation;
            Console.WriteLine($"Result cost {page.Charge}");

        } while (continuationToken != null);

        Console.WriteLine($"Total Charge {totalCharge} RU's");
    }

    public async Task FullPageNumberSpecificationAsync(int age)
    {
        double totalCharge = 0;

        UsersOrderByAgeOffsetSpecification specification = new(age);
        IPageQueryResult<Person> page = await _repository.QueryAsync(specification);
        while (page.HasNextPage is not null && page.HasNextPage.Value)
        {
            foreach (Person person in page.Items)
            {
                Console.WriteLine(person);
            }

            totalCharge += page.Charge;
            Console.WriteLine($"First 10 results cost {page.Charge}");
            specification.NextPage();
            page = await _repository.QueryAsync(specification);
        }

        Console.WriteLine($"Last results cost {page.Charge}");
        Console.WriteLine($"Total Charge {totalCharge} RU's");
    }

    private class UsersOrderByAgeSpecification : ContinuationTokenSpecification<Person>
    {
        public UsersOrderByAgeSpecification(int age)
        {
            Query.Where(p => p.Age > age)
                .OrderByDescending(p => p.Age)
                .PageSize(10);
        }
    }
    private class UsersOrderByAgeOffsetSpecification : OffsetByPageNumberSpecification<Person>
    {
        public UsersOrderByAgeOffsetSpecification(int age)
        {
            Query.Where(p => p.Age > age)
                .OrderByDescending(p => p.Age)
                .PageSize(10)
                .PageNumber(3);
        }

    }
}

