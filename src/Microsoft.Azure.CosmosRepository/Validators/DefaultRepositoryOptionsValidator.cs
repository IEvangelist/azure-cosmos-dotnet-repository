// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Validators;

class DefaultRepositoryOptionsValidator : IRepositoryOptionsValidator
{
    private const string SingularErrorMessage = "An error was encountered";
    private const string PluralErrorMessage = "Multiple errors were encountered";

    public void ValidateForContainerCreation(IOptions<RepositoryOptions> options)
    {
        RepositoryOptions? current = options.Value;

        if (current is null)
        {
            throw new ArgumentNullException(nameof(options), "Repository option are required");
        }

        List<Exception> exceptionsEncountered = [];

        (var connectionStringIsNull, var accountEndpointIsNull, var tokenCredentialIsNull, var databaseIdIsNull, var containerIdIsNull, var containerPerType) = (
            current.CosmosConnectionString is null,
            current.AccountEndpoint is null,
            current.TokenCredential is null,
            string.IsNullOrWhiteSpace(current.DatabaseId),
            string.IsNullOrWhiteSpace(current.ContainerId),
            current.ContainerPerItemType
        );

        // Fatal configuration errors
        if (connectionStringIsNull && tokenCredentialIsNull && accountEndpointIsNull)
        {
            exceptionsEncountered.Add(new ArgumentException($"Missing required arguments, you must provide either a {nameof(current.CosmosConnectionString)} or a {nameof(current.TokenCredential)} and {nameof(current.AccountEndpoint)}."));
        }

        if (!connectionStringIsNull && !tokenCredentialIsNull && !accountEndpointIsNull)
        {
            exceptionsEncountered.Add(new ArgumentException($"Invalid configuration, you must provide either a {nameof(current.CosmosConnectionString)} or a {nameof(current.TokenCredential)} and {nameof(current.AccountEndpoint)}, not both."));
        }

        // Connection string authentication configuration errors - details for another authentication type also provided
        if (!connectionStringIsNull && (!tokenCredentialIsNull || !accountEndpointIsNull))
        {
            exceptionsEncountered.Add(new ArgumentException($"When {current.CosmosConnectionString} is provided, you should not provide {nameof(current.TokenCredential)} or {nameof(current.AccountEndpoint)}"));
        }

        // Token authentication configuration errors - missing required arguments
        if (!tokenCredentialIsNull && accountEndpointIsNull)
        {
            exceptionsEncountered.Add(new ArgumentNullException($"An {nameof(current.AccountEndpoint)} is required when {nameof(current.TokenCredential)} is set."));
        }

        if (tokenCredentialIsNull && !accountEndpointIsNull)
        {
            exceptionsEncountered.Add(new ArgumentNullException($"A {nameof(current.TokenCredential)} is required when {nameof(current.AccountEndpoint)} is set."));
        }

        // Handle fatal, connection string authentication, and token authentication errors
        if (exceptionsEncountered.Count > 0)
        {
            throw new AggregateException(exceptionsEncountered.Count > 1 ? PluralErrorMessage : SingularErrorMessage, exceptionsEncountered);
        }

        // Generic configuration errors
        if (containerPerType)
        {
            return;
        }

        if (databaseIdIsNull)
        {
            throw new ArgumentNullException($"The {nameof(current.DatabaseId)} is required when container per item type is false.");
        }

        if (containerIdIsNull)
        {
            throw new ArgumentNullException($"The {nameof(current.ContainerId)} is required when container per item type is false.");
        }
    }
}