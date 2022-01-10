// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.Exceptions
{
    /// <summary>
    /// An exception stating that the mix of verifyEtag and the Etag value on the item were incompatible.
    /// <remarks>
    /// Please ensure your Item implementation implements IItemWithEtag.
    /// </remarks>
    /// </summary>
    public class InvalidEtagConfigurationException : Exception
    {
        /// <summary>
        /// Constructor specifying the message to set in the exception.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public InvalidEtagConfigurationException(string message) : base(message)
        {
        }
    }
}