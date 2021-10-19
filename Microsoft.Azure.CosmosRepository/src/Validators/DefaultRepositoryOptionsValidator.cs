// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Validators
{
    class DefaultRepositoryOptionsValidator : IRepositoryOptionsValidator
    {
        public void ValidateForContainerCreation(IOptions<RepositoryOptions> options)
        {
            try
            {
                RepositoryOptions current = options?.Value;
                List<Exception> exceptionsEncountered = new();

                if (current is null)
                {
                    throw new AggregateException("An error was encountered", new ArgumentNullException(nameof(options), "Repository option are required"));
                }

                if (current.CosmosConnectionString is null && current.TokenCredential is null)
                {
                    exceptionsEncountered.Add(new ArgumentNullException($"A {nameof(current.CosmosConnectionString)} or {nameof(current.TokenCredential)} are required."));
                }

                if (current.TokenCredential is not null && current.AccountEndpoint is null)
                {
                    exceptionsEncountered.Add(new ArgumentNullException($"The {nameof(current.AccountEndpoint)} is required when using token authentication."));
               }

                if (current.ContainerPerItemType)
                {
                    if (exceptionsEncountered.Count > 0)
                    {
                        throw new AggregateException($"{(exceptionsEncountered.Count > 1 ? "Multiple errors were" : "An error was")} encountered", exceptionsEncountered);
                    }
                    else
                    {
                        return;
                    }
                }

                if (current.DatabaseId is null)
                {
                    exceptionsEncountered.Add(new ArgumentNullException($"The {nameof(current.DatabaseId)} is required when container per item type is false."));
                }

                if (current.ContainerId is null)
                {
                    exceptionsEncountered.Add(new ArgumentNullException($"The {nameof(current.ContainerId)} is required when container per item type is false."));
                }

                if (exceptionsEncountered.Count > 0)
                {
                    throw new AggregateException($"{(exceptionsEncountered.Count > 1 ? "Multiple errors were" : "An error was")} encountered", exceptionsEncountered);
                }
            }
            catch (AggregateException aggregateException)
            {
                foreach(Exception exception in aggregateException.InnerExceptions)
                {
                    throw;
                }
            }
        }
    }
}