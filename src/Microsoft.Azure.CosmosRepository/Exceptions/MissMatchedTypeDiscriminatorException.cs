// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Exceptions;

/// <summary>
/// This exception is thrown when the type field does not match the <see cref="IItem"/>'s type.
/// </summary>
public class MissMatchedTypeDiscriminatorException : Exception
{
    /// <summary>
    /// The current type that was present on the <see cref="IItem"/>
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// The expected type value
    /// </summary>
    public string ExpectedType { get; }

    /// <summary>
    /// Creates a <see cref="MissMatchedTypeDiscriminatorException"/>
    /// </summary>
    /// <param name="type">The current type on the <see cref="IItem"/></param>
    /// <param name="expectedType">The expected type of the <see cref="IItem"/></param>
    public MissMatchedTypeDiscriminatorException(
        string type,
        string expectedType) : base(
        $"The IItem has the type discriminator of {type} and it expected {expectedType}")
    {
        Type = type;
        ExpectedType = expectedType;
    }
}