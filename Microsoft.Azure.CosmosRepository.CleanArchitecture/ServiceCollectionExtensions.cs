// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection WithEtagMappedRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped(typeof(IEtagMappedRepository<,>), typeof(EtagMappedRepository<,>));
        return serviceCollection;
    }
}