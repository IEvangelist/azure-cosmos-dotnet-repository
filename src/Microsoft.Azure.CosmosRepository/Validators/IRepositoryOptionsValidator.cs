// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Validators;

interface IRepositoryOptionsValidator
{
    void ValidateForContainerCreation(IOptions<RepositoryOptions> options);
}