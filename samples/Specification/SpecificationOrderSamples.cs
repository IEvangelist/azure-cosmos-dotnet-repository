// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;
using Microsoft.Azure.CosmosRepository.Specification.Builder;

namespace Specification;

public class SpecificationOrderSamples
{
    private readonly IRepository<Person> _repository;

    public SpecificationOrderSamples(IRepository<Person> repository)
    {
        _repository = repository;
    }
    public async Task BasicOrderAsync()
    {
        OrderByNameSpecification specification = new();
        IQueryResult<Person> result = await _repository.GetAsync(specification);

        Console.WriteLine($"Simple order first result {result.Items.First().Name}");
        Console.WriteLine($"Total Charge {result.Charge} RU's");
    }

    /// <summary>
    /// Required a composite index to be set in the database. Do not know how this is done in the repository pattern
    /// Composite index:
    /// "compositeIndexes":[  
    /// [
    ///     {  
    ///         "path":"/name",
    ///         "order":"ascending"
    ///     },
    ///     {  
    ///         "path":"/age",
    ///         "order":"descending"
    ///     }
    /// ]
    /// ]
    /// </summary>
    /// <returns></returns>
    public async Task MultipleOrderByAsync()
    {
        OrderByMultipleFieldsSpecification specification = new();
        IQueryResult<Person> result = await _repository.GetAsync(specification);

        Console.WriteLine($"Multiple order first result {result.Items.First().Name}");
        Console.WriteLine($"Total Charge {result.Charge} RU's");
    }

    private class OrderByNameSpecification : ListSpecification<Person>
    {
        public OrderByNameSpecification()
        {
            Query.OrderBy(x => x.Name);
        }
    }


    private class OrderByMultipleFieldsSpecification : ListSpecification<Person>
    {
        public OrderByMultipleFieldsSpecification()
        {
            Query.OrderByDescending(p => p.Name)
                .ThenBy(p => p.Age);
        }
    }
}
