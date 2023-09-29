// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Exceptions;

/// <summary>
/// This exception is thrown when the type field does not match the <see cref="IItem"/>'s type.
/// </summary>
/// <remarks>
/// Creates a <see cref="MissMatchedTypeDiscriminatorException"/>
/// </remarks>
/// <param name="type">The current type on the <see cref="IItem"/></param>
/// <param name="expectedType">The expected type of the <see cref="IItem"/></param>
public class MissMatchedTypeDiscriminatorException(
    string type,
    string expectedType) : Exception(
    $"The IItem has the type discriminator of {type} and it expected {expectedType}")
{
    /// <summary>
    /// The current type that was present on the <see cref="IItem"/>
    /// </summary>
    public string Type { get; } = type;

    /// <summary>
    /// The expected type value
    /// </summary>
    public string ExpectedType { get; } = expectedType;
}