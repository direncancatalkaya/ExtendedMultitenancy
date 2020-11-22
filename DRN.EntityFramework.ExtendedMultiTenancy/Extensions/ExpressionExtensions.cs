using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFramework.ExtensionUtilities.Extensions
{
    public static class ExpressionUtilities
    {
        internal static void AddQueryFilter<T>(this EntityTypeBuilder entityTypeBuilder,
            Expression<Func<T, bool>> expression)
        {
            var parameterType = Expression.Parameter(entityTypeBuilder.Metadata.ClrType);
            var expressionFilter = ReplacingExpressionVisitor.Replace(
                expression.Parameters.Single(), parameterType, expression.Body);

            var currentQueryFilter = entityTypeBuilder.Metadata.GetQueryFilter();
            if (currentQueryFilter != null)
            {
                var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
                    currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
                expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
            }

            var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);
            entityTypeBuilder.HasQueryFilter(lambdaExpression);
        }

        internal static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1,
            Expression<Func<T, bool>> expression2)
        {
            var body = Expression.AndAlso(expression1.Body, expression2.Body);

            return Expression.Lambda<Func<T, bool>>(body, expression1.Parameters[0]);
        }

        // public static IQueryable<TEntity> IgnoreSoftDeleteFilter<TEntity>(
        //     this IQueryable<TEntity> baseQuery, string currentTenantId)
        //     where TEntity : class, ITenant<>
        // {
        //     return baseQuery.IgnoreQueryFilters()
        //         .Where(x => x.TenantId == currentTenantId)
        // }
    }
}