// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Services.Products.Domain.Enums;

namespace Services.Products.Domain.Entities;

public class InventoryAudit : Item
{
    public InventoryAction InventoryAction { get; set; }

    public string ProductId { get; set; }

    public int Amount { get; set; }

    public DateTime OccuredAtUtc { get; set; }

    public string CustomerId { get; set; }

    public InventoryAudit(InventoryAction inventoryAction, string productId, int amount, string customerId)
    {
        InventoryAction = inventoryAction;
        ProductId = productId;
        Amount = amount;
        OccuredAtUtc = DateTime.UtcNow;
        CustomerId = customerId;
    }
}