// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Services.Products.Domain.Entities;

namespace Services.Products.Application.Abstractions;

public interface IProductsRepository
{
    ValueTask Create(Product product);
}