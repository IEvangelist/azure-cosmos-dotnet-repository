// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Services.Products.Application.Abstractions;
using Services.Products.Application.Services;
using Services.Products.Domain.Entities;

namespace Services.Products.Infrastructure.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly IRepository<Product> _repository;

    public ProductsRepository(IRepository<Product> repository) =>
        _repository = repository;

    public async ValueTask Create(Product product) =>
        await _repository.CreateAsync(product);
}