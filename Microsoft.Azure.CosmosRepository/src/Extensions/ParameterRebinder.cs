// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosRepository.Extensions
{
    internal class ParameterRebinder : ExpressionVisitor
    {
        readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        internal ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map) =>
            _map = map ?? new();

        internal static Expression ReplaceParameters(
            Dictionary<ParameterExpression, ParameterExpression> map, Expression exp) =>
            new ParameterRebinder(map).Visit(exp);

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            if (_map.TryGetValue(parameter, out ParameterExpression replacement))
            {
                parameter = replacement;
            }

            return base.VisitParameter(parameter);
        }
    }
}