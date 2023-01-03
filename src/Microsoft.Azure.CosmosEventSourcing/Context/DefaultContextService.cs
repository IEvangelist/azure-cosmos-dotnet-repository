// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace System.Runtime.CompilerServices.Context;

/// <inheritdoc />
public class DefaultContextService : IContextService
{
    /// <inheritdoc />
    public string? CorrelationId { get; set; }
}