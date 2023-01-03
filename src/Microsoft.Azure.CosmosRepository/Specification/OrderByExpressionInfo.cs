// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Specification;

/// <summary>
///
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class OrderExpressionInfo<TItem>
    where TItem : IItem
{
    private readonly Lazy<Func<TItem, object>> _keySelectorFunc;

    /// <summary>
    /// Creates instance of <see cref="OrderExpressionInfo{TItem}" />.
    /// </summary>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="orderType">Whether to (subsequently) sort ascending or descending.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="keySelector"/> is null.</exception>
    public OrderExpressionInfo(Expression<Func<TItem, object>> keySelector, OrderTypeEnum orderType)
    {
        _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

        KeySelector = keySelector;
        OrderType = orderType;

        _keySelectorFunc = new Lazy<Func<TItem, object>>(KeySelector.Compile);
    }

    /// <summary>
    /// A function to extract a key from an element.
    /// </summary>
    public Expression<Func<TItem, object>> KeySelector { get; }

    /// <summary>
    /// Whether to (subsequently) sort ascending or descending.
    /// </summary>
    public OrderTypeEnum OrderType { get; }

    /// <summary>
    /// Compiled <see cref="KeySelector" />.
    /// </summary>
    public Func<TItem, object> KeySelectorFunc => _keySelectorFunc.Value;
}
