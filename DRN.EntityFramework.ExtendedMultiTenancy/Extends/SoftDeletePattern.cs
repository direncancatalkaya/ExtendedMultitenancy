using System;
using System.Linq.Expressions;
using System.Reflection;
using EntityFramework.ExtensionUtilities.Entities.Abstract;
using EntityFramework.ExtensionUtilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFramework.ExtensionUtilities.Extends
{
    public static class SoftDeletePattern
    {
        private static readonly MethodInfo ConfigureSoftDeleteMethodInfo =
            typeof(SoftDeletePattern).GetMethod(nameof(ConfigureSoftDeleteFilters),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

        public static void ApplySoftDeletePattern<TUserId>(this ModelBuilder modelBuilder, TUserId userId)
        {
            var entityTypeList = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in entityTypeList)
                ConfigureSoftDeleteMethodInfo
                    .MakeGenericMethod(entityType.ClrType, userId.GetType())
                    .Invoke(null, new object[] {modelBuilder, entityType});
        }

        private static void ConfigureSoftDeleteFilters<TEntity, TUserId>(ModelBuilder modelBuilder,
            IMutableEntityType entityType) where TEntity : class where TUserId : struct
        {
            if (!ShouldFilterSoftDelete<TUserId>(entityType)) return;
            var softDeleteExpression = CreateSoftDeleteExpression<TEntity, TUserId>();
            if (softDeleteExpression != null) modelBuilder.Entity<TEntity>().AddQueryFilter(softDeleteExpression);
        }

        private static bool ShouldFilterSoftDelete<TUserId>(IMutableEntityType entityType)
            where TUserId : struct
        {
            return typeof(ISoftDelete<TUserId>).IsAssignableFrom(entityType.ClrType);
        }

        private static Expression<Func<TEntity, bool>> CreateSoftDeleteExpression<TEntity, TUserId>()
            where TEntity : class
            where TUserId : struct
        {
            Expression<Func<TEntity, bool>> removedFilter = e => ((ISoftDelete<TUserId>) e).IsDeleted == false;
            return removedFilter;
        }
    }
}