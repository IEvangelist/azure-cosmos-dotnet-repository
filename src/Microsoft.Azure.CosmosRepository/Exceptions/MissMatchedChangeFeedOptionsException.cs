// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Exceptions;

/// <summary>
/// An exception stating that <see cref="IItem"/>'s sharing a container have configured different change feed options.
/// </summary>
public class MissMatchedChangeFeedOptionsException : Exception
{
    /// <summary>
    /// The types of <see cref="IItem"/>'s which are sharing a container.
    /// </summary>
    public IReadOnlyList<Type> ItemTypes { get; }

    /// <summary>
    /// Creates a <see cref="MissMatchedChangeFeedOptionsException"/>
    /// </summary>
    /// <param name="message">The message detailing the miss match.</param>
    /// <param name="itemTypes">The types of <see cref="IItem"/>'s that are miss matched.</param>
    public MissMatchedChangeFeedOptionsException(string message, IReadOnlyList<Type> itemTypes) : base(message)
    {
        ItemTypes = itemTypes;
    }
}