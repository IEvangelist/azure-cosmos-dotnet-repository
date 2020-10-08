using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.CosmosRepository.Attributes
{
    /// <summary>
    /// Allows for decorating an Item with a specified PartitionKeyPath in the case of wanting a container per item.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class)]
    public class PartitionKeyPathAttribute : Attribute
    {
        /// <summary>
        /// The PartitionKeyPath.
        /// </summary>
        public string PartitionKeyPath { get; }

        /// <summary>
        /// Instantiates the attribute with the given partitionKeyPath
        /// </summary>
        /// <param name="partitionKeyPath"></param>
        /// <exception cref="ArgumentNullException">Throws if no partitionKeyPath string is specified.</exception>
        public PartitionKeyPathAttribute(string partitionKeyPath)
        {
            PartitionKeyPath = !string.IsNullOrWhiteSpace(partitionKeyPath) ? partitionKeyPath : throw new ArgumentNullException(nameof(partitionKeyPath));
        }
    }
}