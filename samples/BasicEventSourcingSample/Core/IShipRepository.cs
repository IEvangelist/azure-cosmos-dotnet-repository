// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace BasicEventSourcingSample.Core;

public interface IShipRepository
{
    ValueTask CreateShip(Ship ship);
}