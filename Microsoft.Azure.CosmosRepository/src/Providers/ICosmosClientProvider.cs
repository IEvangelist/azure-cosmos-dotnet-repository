// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Microsoft.Azure.CosmosRepositoryTests")]

namespace Microsoft.Azure.CosmosRepository.Providers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// The cosmos client provider exposes a means of providing an instance to the configured <see
    /// cref="CosmosClient" /> object, which is shared.
    /// </summary>
    internal interface ICosmosClientProvider
    {
        Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume);
    }
}
