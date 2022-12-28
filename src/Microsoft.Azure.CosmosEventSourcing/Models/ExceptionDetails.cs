// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Models;

/// <summary>
/// The details of the exception that cased the processing to fail.
/// </summary>
/// <param name="Type">The type of the <see cref="Exception"/></param>
/// <param name="Message">The message provided by the <see cref="Exception"/></param>
public record ExceptionDetails(
    string Type,
    string Message);