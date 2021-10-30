// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Services.Products.Application.Commands;

public record CreateProduct(string Name, string Description, string ImageUrl, string ProductCategoryId);