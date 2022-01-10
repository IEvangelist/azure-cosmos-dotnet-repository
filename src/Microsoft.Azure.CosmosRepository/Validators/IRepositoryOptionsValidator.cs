// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Validators
{
    interface IRepositoryOptionsValidator
    {
        void ValidateForContainerCreation(IOptions<RepositoryOptions> options);
    }
}