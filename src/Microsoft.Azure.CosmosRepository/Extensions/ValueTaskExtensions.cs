// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.Extensions
{
    /// <summary>
    /// A set of useful extension methods for a <see cref="ValueTask{TResult}"/>
    /// </summary>
    public static class ValueTaskExtensions
    {
        /// <summary>
        /// Converts a <see cref="ValueTask"/> of <see cref="IEnumerable{T}"/> to a <see cref="List{T}"/>
        /// </summary>
        /// <param name="valueTask">The value task</param>
        /// <typeparam name="T">The type of <see cref="IEnumerable{T}"/></typeparam>
        /// <returns></returns>
        public static async ValueTask<List<T>> ToListAsync<T>(this ValueTask<IEnumerable<T>> valueTask)
        {
            IEnumerable<T> e = await valueTask;
            return e.ToList();
        }
    }
}