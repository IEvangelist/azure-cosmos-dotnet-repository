using System.Collections.Generic;

namespace Microsoft.Azure.CosmosRepository.Pagination
{
    /// <summary>
    /// List of items
    /// </summary>
    public class ListResult<T>
    {
        /// <summary>
        /// Creates a new <see cref="ListResult{T}"/> object.
        /// </summary>
        public ListResult() { }

        /// <summary>
        /// Creates a new <see cref="ListResult{T}"/> object.
        /// </summary>
        /// <param name="items">List of items</param>
        public ListResult(IReadOnlyList<T> items)
        {
            Items = items;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> Items
        {
            get { return _items ??= new List<T>(); }
            set { _items = value; }
        }

        private IReadOnlyList<T> _items;
    }
}
