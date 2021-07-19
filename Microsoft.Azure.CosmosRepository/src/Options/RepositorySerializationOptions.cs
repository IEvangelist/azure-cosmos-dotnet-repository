// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Options
{
    /// <summary>
    /// The serialization options for the Cosmos DB repository.
    /// These are mapped to the <see cref="CosmosClientOptions.SerializerOptions"/> object.
    /// </summary>
    public class RepositorySerializationOptions
    {
        /// <summary>
        /// Gets or sets if the serializer should ignore null properties.
        /// </summary>
        /// <remarks>The default value is false</remarks>
        public bool IgnoreNullValues { get; set; }

        /// <summary>
        /// Gets or sets if the serializer should use indentation.
        /// </summary>
        /// <remarks>The default value is false</remarks>
        public bool Indented { get; set; }

        /// <summary>
        /// Gets or sets whether the naming policy used to convert a string-based name to
        /// another format, such as a camel-casing format.
        /// </summary>
        /// <remarks>The default value is <see cref="CosmosPropertyNamingPolicy.CamelCase"/>.</remarks>
        public CosmosPropertyNamingPolicy PropertyNamingPolicy { get; set; } = CosmosPropertyNamingPolicy.CamelCase;
    }
}