// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Specification
{
    /// <inheritdoc cref="ISpecificationBuilder{T}"/>
    public interface IOrderedSpecificationBuilder<T> : ISpecificationBuilder<T>
        where T : IItem
    {
    }
}
