// Copyright © IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Properties;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Microsoft.Azure.CosmosRepository
{
	/// <inheritdoc/>
	internal class DefaultRepositoryFactory : IRepositoryFactory
	{
		readonly IServiceProvider _serviceProvider;

		/// <summary>
		/// Constructor for the default respositroy factory.
		/// </summary>
		/// <param name="serviceProvider"></param>
		public DefaultRepositoryFactory(IServiceProvider serviceProvider) =>
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(
				nameof(serviceProvider), Resources.AServiceProviderInstanceIsRequired);

		/// <inheritdoc/>
		public IRepository<TItem> RepositoryOf<TItem>()
			where TItem : Item =>
			_serviceProvider.GetRequiredService<IRepository<TItem>>();

	}
}
