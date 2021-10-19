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
        private const string SingularErrorMessage = "An error was encountered";
        private const string PluralErrorMessage = "Multiple errors were encountered";

        public void ValidateForContainerCreation(IOptions<RepositoryOptions> options)
        {
            try
            {
                RepositoryOptions current = options?.Value;
                List<Exception> exceptionsEncountered = new();

                bool respositoryOptionsIsNull = current is null;

                if (respositoryOptionsIsNull)
                {
                    throw new AggregateException(SingularErrorMessage, new ArgumentNullException(nameof(options), "Repository option are required"));
                }

                (bool connectionStringIsNull, bool accountEndpointIsNull, bool tokenCredentialIsNull, bool databaseIdIsNull, bool containerIdIsNull) = (
                    current.CosmosConnectionString is null,
                    current.AccountEndpoint is null,
                    current.TokenCredential is null,
                    current.DatabaseId is null,
                    current.ContainerId is null
                );

                // Bad - nothing provided for either connection method
                if (connectionStringIsNull && tokenCredentialIsNull && accountEndpointIsNull)
                {
                    exceptionsEncountered.Add(new ArgumentException($"Missing required arguments, you must provide either a {nameof(current.CosmosConnectionString)} or a {nameof(current.TokenCredential)} and {nameof(current.AccountEndpoint)}."));
                }

                // Bad - everything provided for both connection methods
                if (!connectionStringIsNull && !tokenCredentialIsNull && !accountEndpointIsNull)
                {
                    exceptionsEncountered.Add(new ArgumentException($"Invalid configuration, you must provide either a {nameof(current.CosmosConnectionString)} or a {nameof(current.TokenCredential)} and {nameof(current.AccountEndpoint)}, not both."));
                }

                // Bad - some token auth details provided along connection string, the reverse case is covered above with everything for both connection methods
                if (!connectionStringIsNull && (!tokenCredentialIsNull || !accountEndpointIsNull))
                {
                    exceptionsEncountered.Add(new ArgumentException($"When {current.CosmosConnectionString} is provided, you should not provide {nameof(current.TokenCredential)} or {nameof(current.AccountEndpoint)}"));
                }

                // Bad - endpoint without token
                if (!tokenCredentialIsNull && accountEndpointIsNull)
                {
                    exceptionsEncountered.Add(new ArgumentNullException($"An {nameof(current.AccountEndpoint)} is required when {nameof(current.TokenCredential)} is set."));
                }

                // Bad - token without endpoint
                if (tokenCredentialIsNull && !accountEndpointIsNull)
                {
                    exceptionsEncountered.Add(new ArgumentNullException($"A {nameof(current.TokenCredential)} is required when {nameof(current.AccountEndpoint)} is set."));
                }

                if (current.ContainerPerItemType)
                {
                    if (exceptionsEncountered.Count > 0)
                    {
                        throw new AggregateException(exceptionsEncountered.Count > 1 ? PluralErrorMessage : SingularErrorMessage, exceptionsEncountered);
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
                    throw new AggregateException(exceptionsEncountered.Count > 1 ? PluralErrorMessage : SingularErrorMessage, exceptionsEncountered);
                }
            }
            catch (AggregateException aggregateException)
            {
                foreach (Exception exception in aggregateException.InnerExceptions)
                {
                    throw;
                }
            }
        }
    }
}