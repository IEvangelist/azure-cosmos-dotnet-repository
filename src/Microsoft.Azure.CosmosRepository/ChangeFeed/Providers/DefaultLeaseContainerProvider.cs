// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;

class DefaultLeaseContainerProvider : ILeaseContainerProvider
{
    private readonly ICosmosClientProvider _cosmosClientProvider;
    private readonly Lazy<Task<Container>> _lazyContainer;
    private readonly IOptionsMonitor<RepositoryOptions> _optionsMonitor;
    private const string LeaseContainerName = "lease";
    private const string LeastContainerPartitionKey = "/id";

    public DefaultLeaseContainerProvider(
        ICosmosClientProvider cosmosClientProvider,
        IOptionsMonitor<RepositoryOptions> optionsMonitor)
    {
        _cosmosClientProvider = cosmosClientProvider;
        _optionsMonitor = optionsMonitor;
        _lazyContainer = new Lazy<Task<Container>>(BuildLeaseContainer);
    }

    public Task<Container> GetLeaseContainerAsync() =>
        _lazyContainer.Value;

    private async Task<Container> BuildLeaseContainer()
    {
        RepositoryOptions repositoryOptions = _optionsMonitor.CurrentValue;

        Database database =
            repositoryOptions.IsAutoResourceCreationIfNotExistsEnabled
                ? await _cosmosClientProvider.UseClientAsync(
                        client => client.CreateDatabaseIfNotExistsAsync(repositoryOptions.DatabaseId))
                    .ConfigureAwait(false)
                : await _cosmosClientProvider.UseClientAsync(
                        client => Task.FromResult(client.GetDatabase(repositoryOptions.DatabaseId)))
                    .ConfigureAwait(false);

        Container container =
            repositoryOptions.IsAutoResourceCreationIfNotExistsEnabled
                ? await database.CreateContainerIfNotExistsAsync(LeaseContainerName, LeastContainerPartitionKey)
                    .ConfigureAwait(false)
                : await Task.FromResult(database.GetContainer(LeaseContainerName));

        return container;
    }
}
