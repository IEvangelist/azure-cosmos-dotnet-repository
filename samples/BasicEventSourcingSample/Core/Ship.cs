// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace BasicEventSourcingSample.Core;

public class Ship
{
    public string Name { get; }

    public DateTime Commissioned { get; }
    public DateTime CreatedAt { get; }

    public Ship(string name, DateTime commissioned, DateTime? createdAt = null)
    {
        Name = name;
        Commissioned = commissioned;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }
}