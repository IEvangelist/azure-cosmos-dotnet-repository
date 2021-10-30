// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Services.Products.Application.Abstractions;
using Services.Products.Application.Commands;
using Services.Shared;

namespace Services.Products.Application.Services;

public class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;
    private readonly ICategoriesRepository _categoriesRepository;

    public ProductsService(IProductsRepository productsRepository, ICategoriesRepository categoriesRepository)
    {
        _productsRepository = productsRepository;
        _categoriesRepository = categoriesRepository;
    }

    public async ValueTask<ServiceResult> CreateProduct(CreateProduct createProduct)
    {
        if (await _categoriesRepository.Exists(createProduct.ProductCategoryId) is false)
        {
            return ServiceResult.Failure($"A product category with the id {createProduct.ProductCategoryId} does not exist");
        }



        return ServiceResult.Successful();
    }
}