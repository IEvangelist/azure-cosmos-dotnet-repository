// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using WebTier.Models;

namespace WebTier.Integration.Tests;

public class LanguageDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; }

    public string[] Aliases { get; set; }

    public string Description { get; set; }

    public ProgrammingStyle PrimaryStyle { get; set; }

    public DateTime InitialReleaseDate { get; set; }
}