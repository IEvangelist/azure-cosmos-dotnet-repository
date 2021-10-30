// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace Services.Products.Domain.Entities;

public class StockInventory : Item
{
    public int CurrentStockCount { get; set; }

    public int CurrentStockOnOrder { get; set; }

    public StockInventory(int currentStockCount, int currentStockOnOrder)
    {
        CurrentStockCount = currentStockCount;
        CurrentStockOnOrder = currentStockOnOrder;
    }

    public StockInventory()
    {

    }
}