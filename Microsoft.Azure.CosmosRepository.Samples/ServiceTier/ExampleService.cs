// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace ServiceTier
{
    public class ExampleService : IExampleService
    {
        readonly IRepository<Person> _personRepository;

        public ExampleService(IRepository<Person> personRepository) =>
            _personRepository = personRepository;
    }
}
