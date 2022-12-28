// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

[assembly: InternalsVisibleTo("Microsoft.Azure.CosmosRepositoryTests")]
namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// The cosmos client provider exposes a means of providing
/// an instance to the configured <see cref="CosmosClient"/> object,
/// which is shared.
/// </summary>
interface ICosmosClientProvider
{
    Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume);
}
