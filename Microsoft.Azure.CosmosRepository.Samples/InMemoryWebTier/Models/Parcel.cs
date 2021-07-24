// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository;

namespace InMemoryWebTier.Models
{
    public class Parcel : Item
    {
        public string Barcode { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime PromisedBy { get; set; }

        public List<ParcelItem> Items { get; set; }

        protected override string GetPartitionKeyValue()
        {
            return Barcode;
        }
    }
}