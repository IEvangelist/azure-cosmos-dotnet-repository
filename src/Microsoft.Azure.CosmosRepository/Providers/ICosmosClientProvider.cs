// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

[assembly: InternalsVisibleTo("Microsoft.Azure.CosmosRepositoryTests")]
namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <summary>
    /// The cosmos client provider exposes a means of providing
    /// an instance to the configured <see cref="CosmosClient"/> object,
    /// which is shared.
    /// </summary>
    interface ICosmosClientProvider
    {
        Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume);
    }
}
