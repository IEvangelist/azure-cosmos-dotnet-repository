// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Configuration.Internal;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.CosmosRepository.Builders
{
    /// <summary>
    /// Allows metadata such etags and last updated times to be added to an <see cref="IItem"/>
    /// </summary>
    public class ItemOptionsBuilder
    {
        internal ItemOptionsBuilder(Type type)
        {
            Type = type;
            ETagPropertyInformation = null;
        }
        
        internal Type Type { get; private set; }

        internal PropertyInfo ETagPropertyInformation { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fieldOrPropertyName"></param>
        /// <returns></returns>
        public ItemOptionsBuilder WithETag(string fieldOrPropertyName)
        {
            PropertyInfo propertyInfo = Type.GetProperty(fieldOrPropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            ETagPropertyInformation = propertyInfo ?? throw new InvalidOperationException(
                $"A property or field with the name {fieldOrPropertyName} could not be found on the root of the object with the type {Type.Name}");
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertyLambda"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ItemOptionsBuilder WithETag<TProperty>(Expression<Func<IItem, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            return WithETag(propInfo.Name);
        }
    }
}