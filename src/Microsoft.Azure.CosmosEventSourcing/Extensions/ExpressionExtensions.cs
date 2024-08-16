// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

internal static class ExpressionExtensions
{
    internal static Expression<T> Compose<T>(
        this Expression<T> first,
        Expression<T> second,
        Func<Expression, Expression, Expression> merge)
    {
        var map =
            first.Parameters
                .Select((parameter, index) => (parameter, second: second.Parameters[index]))
                .ToDictionary(p => p.second, p => p.parameter);

        Expression secondBody = ParameterReBinder.ReplaceParameters(map, second.Body);

        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    private class ParameterReBinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        private ParameterReBinder(Dictionary<ParameterExpression, ParameterExpression> map) =>
            _map = map ?? [];

        internal static Expression ReplaceParameters(
            Dictionary<ParameterExpression, ParameterExpression> map, Expression exp) =>
            new ParameterReBinder(map).Visit(exp)!;

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            if (_map.TryGetValue(parameter, out ParameterExpression? replacement))
            {
                parameter = replacement;
            }

            return base.VisitParameter(parameter);
        }
    }
}