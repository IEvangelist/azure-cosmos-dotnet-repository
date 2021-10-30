// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Services.Products.Application.Abstractions;

public interface ICategoriesRepository
{
    ValueTask<bool> Exists(string id);
}