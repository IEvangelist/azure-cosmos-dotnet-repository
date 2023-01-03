// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository;

namespace InMemoryWebTier.Models;

public class Parcel : Item
{
    public Guid CustomerId { get; set; }

    public DateTime PromisedBy { get; set; }

    public List<ParcelItem> Items { get; set; }
}
