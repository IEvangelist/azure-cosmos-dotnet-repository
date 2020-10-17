// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace WebTier.Models
{
    using System;
    using Microsoft.Azure.CosmosRepository;

    public class Language : Item
    {
        public string[] Aliases { get; set; }

        public string Description { get; set; }

        public DateTime InitialReleaseDate { get; set; }

        public string Name { get; set; }

        public ProgrammingStyle PrimaryStyle { get; set; }
    }
}
