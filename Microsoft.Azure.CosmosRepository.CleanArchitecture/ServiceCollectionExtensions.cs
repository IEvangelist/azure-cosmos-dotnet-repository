// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection WithEtagMappedRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient(typeof(IEtagMappedRepository<,>), typeof(EtagMappedRepository<,>));
        serviceCollection.AddTransient<IEtagCache, EtagCache>();
        return serviceCollection;
    }
}