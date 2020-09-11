// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Options
{
    /// <summary>
    /// A repository options class, representing 
    /// various Azure Cosmos DB configuration settings.
    /// </summary>
    public class RepositoryOptions
    {
        /// <summary>
        /// Gets or sets the cosmos connection string. Primary or secondary connection strings are valid.
        /// </summary>
        public string CosmosConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name identifier for the cosmos database.
        /// </summary>
        public string DatabaseId { get; set; }

        /// <summary>
        /// Gets or sets the name identifier for the cosmos container that corresponds to the <see cref="DatabaseId"/>.
        /// </summary>        
        public string ContainerId { get; set; }
    }
}