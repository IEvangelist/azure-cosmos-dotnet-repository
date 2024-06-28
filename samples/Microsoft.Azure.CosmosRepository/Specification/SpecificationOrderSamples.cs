// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Specification;

public class SpecificationOrderSamples(IRepository<Person> repository)
{
    public async Task BasicOrderAsync()
    {
        OrderByNameSpecification specification = new();
        IQueryResult<Person> result = await repository.QueryAsync(specification);

        Console.WriteLine($"Simple order first result {result.Items[0].Name}");
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
        IQueryResult<Person> result = await repository.QueryAsync(specification);

        Console.WriteLine($"Multiple order first result {result.Items[0].Name}");
        Console.WriteLine($"Total Charge {result.Charge} RU's");
    }

    private class OrderByNameSpecification : DefaultSpecification<Person>
    {
        public OrderByNameSpecification()
        {
            Query.OrderBy(x => x.Name);
        }
    }


    private class OrderByMultipleFieldsSpecification : DefaultSpecification<Person>
    {
        public OrderByMultipleFieldsSpecification()
        {
            Query.OrderByDescending(p => p.Name)
                .ThenBy(p => p.Age);
        }
    }
}
