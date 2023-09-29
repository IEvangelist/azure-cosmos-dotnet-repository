// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Exceptions;

/// <summary>
/// An exception stating that the mix of verifyEtag and the Etag value on the item were incompatible.
/// <remarks>
/// Please ensure your Item implementation implements IItemWithEtag.
/// </remarks>
/// </summary>
/// <remarks>
/// Constructor specifying the message to set in the exception.
/// </remarks>
/// <param name="message">The message for the exception.</param>
public class InvalidEtagConfigurationException(string message) : Exception(message)
{
}