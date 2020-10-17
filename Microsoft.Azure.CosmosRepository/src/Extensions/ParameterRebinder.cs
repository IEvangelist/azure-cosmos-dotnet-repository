// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Extensions
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        internal ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map) =>
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();

        internal static Expression ReplaceParameters(
            Dictionary<ParameterExpression, ParameterExpression> map, Expression exp) =>
            new ParameterRebinder(map).Visit(exp);

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            if (this.map.TryGetValue(parameter, out ParameterExpression replacement))
            {
                parameter = replacement;
            }

            return base.VisitParameter(parameter);
        }
    }
}
