// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Validators
{
    class DefaultRepositoryOptionsValidator : IRepositoryOptionsValidator
    {
        public void ValidateForContainerCreation(IOptions<RepositoryOptions> options)
        {
            RepositoryOptions current = options?.Value
                ?? throw new ArgumentNullException(nameof(options), "Repository option are required");

            if (current.CosmosConnectionString is null && current.TokenCredential is null)
            {
                throw new ArgumentNullException($"A {nameof(current.CosmosConnectionString)} or {nameof(current.TokenCredential)} are required.");
            }

            if (current.TokenCredential is not null && current.AccountEndpoint is null)
            {
                throw new ArgumentNullException($"The {nameof(current.AccountEndpoint)} is required when using token authentication.");
            }

            if (current.ContainerPerItemType)
            {
                return;
            }

            if (current.DatabaseId is null)
            {
                throw new ArgumentNullException($"The {nameof(current.DatabaseId)} is required when container per item type is false.");
            }

            if (current.ContainerId is null)
            {
                throw new ArgumentNullException($"The {nameof(current.ContainerId)} is required when container per item type is false.");
            }
        }
    }
}