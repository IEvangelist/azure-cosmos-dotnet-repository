// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;
using Moq;

namespace Microsoft.Azure.CosmosRepositoryTests.Abstractions
{
    public abstract class WithRepositoryOptions
    {
        protected readonly Mock<IOptions<RepositoryOptions>> _options;
        protected readonly RepositoryOptions _repositoryOptions;

        protected WithRepositoryOptions()
        {
            _options = new Mock<IOptions<RepositoryOptions>>();
            _repositoryOptions = new RepositoryOptions();
            _options.Setup(o => o.Value).Returns(_repositoryOptions);
        }
    }
}