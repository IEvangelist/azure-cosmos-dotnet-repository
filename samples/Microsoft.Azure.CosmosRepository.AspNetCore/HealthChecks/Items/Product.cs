// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace HealthChecks.Items;

/// <summary>
/// Simple product item for demonstrating eager container initialization with health checks.
/// </summary>
public class Product : Item
{
    /// <summary>
    /// The product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The product category (used as partition key).
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The product price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The product description.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
