using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Edreams.OutlookMiddleware.Common.Exceptions;

namespace Edreams.OutlookMiddleware.Common.Extensions
{
    /// <summary>
    /// Extension methods on Expression
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Combines two expressions with an AND operation.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>The combined expression.</returns>
        public static Expression<TDelegate> AndAlso<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right)
        {
            return Expression.Lambda<TDelegate>(Expression.AndAlso(
                 left.Body, new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body)), left.Parameters);
        }

        /// <summary>
        /// Combines two expressions with an OR operation.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <returns>The combined expression.</returns>
        public static Expression<TDelegate> OrElse<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right)
        {
            return Expression.Lambda<TDelegate>(Expression.OrElse(
                left.Body, new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body)), left.Parameters);
        }

        /// <summary>
        /// Gets the name of the specified property.
        /// </summary>
        /// <typeparam name="TObject">The type of the object owning the property.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The property to get the property name for.</param>
        public static string GetPropertyName<TObject, TProperty>(this Expression<Func<TObject, TProperty>> propertyExpression)
        {
            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null) { throw new EdreamsFatalException("Incompatible PropertyExpression encountered!"); }
            return memberExpression.Member.Name;
        }

        private class ExpressionParameterReplacer : ExpressionVisitor
        {
            private readonly IDictionary<ParameterExpression, ParameterExpression> _parameterReplacements;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExpressionParameterReplacer"/> class.
            /// </summary>
            /// <param name="fromParameters">From parameters.</param>
            /// <param name="toParameters">To parameters.</param>
            public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
            {
                _parameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
                for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                {
                    _parameterReplacements.Add(fromParameters[i], toParameters[i]);
                }
            }

            /// <summary>
            /// Visits the <see cref="ParameterExpression" />.
            /// </summary>
            /// <param name="node">The expression to visit.</param>
            /// <returns>The modified expression, if it or any subexpression was modified;
            /// otherwise, returns the original expression.</returns>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                ParameterExpression replacement;
                if (_parameterReplacements.TryGetValue(node, out replacement))
                {
                    node = replacement;
                }
                return base.VisitParameter(node);
            }
        }
    }
}