// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Services.Products.Application.Commands;
using Services.Shared;

namespace Services.Products.Application.Services;

public interface IProductsService
{
    ValueTask<ServiceResult> CreateProduct(CreateProduct createProduct);
}