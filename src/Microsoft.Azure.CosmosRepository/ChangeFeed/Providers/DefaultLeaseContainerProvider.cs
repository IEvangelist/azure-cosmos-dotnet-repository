// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

class DefaultLeaseContainerProvider : ILeaseContainerProvider
{
    private readonly ICosmosClientProvider _cosmosClientProvider;
    private readonly Lazy<Task<Container>> _lazyContainer;
    private readonly RepositoryOptions _repositoryOptions;
    private const string LeaseContainerName = "lease";
    private const string LeastContainerPartitionKey = "/id";

    public DefaultLeaseContainerProvider(
        ICosmosClientProvider cosmosClientProvider,
        IOptionsMonitor<RepositoryOptions> optionsMonitor)
    {
        _repositoryOptions = optionsMonitor.CurrentValue;
        _cosmosClientProvider = cosmosClientProvider;
        _lazyContainer = new Lazy<Task<Container>>(BuildLeaseContainer);
    }

    public Task<Container> GetLeaseContainerAsync() =>
        _lazyContainer.Value;

    private async Task<Container> BuildLeaseContainer()
    {
        Database database =
            _repositoryOptions.IsAutoResourceCreationIfNotExistsEnabled
                ? await _cosmosClientProvider.UseClientAsync(
                        client => client.CreateDatabaseIfNotExistsAsync(_repositoryOptions.DatabaseId))
                    .ConfigureAwait(false)
                : await _cosmosClientProvider.UseClientAsync(
                        client => Task.FromResult(client.GetDatabase(_repositoryOptions.DatabaseId)))
                    .ConfigureAwait(false);

        Container container =
            _repositoryOptions.IsAutoResourceCreationIfNotExistsEnabled
                ? await database.CreateContainerIfNotExistsAsync(LeaseContainerName, LeastContainerPartitionKey)
                    .ConfigureAwait(false)
                : await Task.FromResult(database.GetContainer(LeaseContainerName));

        return container;
    }
}