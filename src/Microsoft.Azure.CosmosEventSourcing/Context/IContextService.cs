// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace System.Runtime.CompilerServices.Context;

/// <summary>
/// Allows context values to be set and retrieved for a given request/scope
/// </summary>
public interface IContextService
{
    /// <summary>
    /// An ID that correlates a request/scope with an event.
    /// </summary>
    string? CorrelationId { get; set; }
}