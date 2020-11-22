using System;
using System.Linq.Expressions;
using System.Reflection;
using EntityFramework.ExtensionUtilities.Entities.Abstract;
using EntityFramework.ExtensionUtilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFramework.ExtensionUtilities.Extends
{
    public static class MultiTenancy
    {
        private static readonly MethodInfo ConfigureMultiTenancyMethodInfo =
            typeof(MultiTenancy).GetMethod(nameof(ConfigureMultiTenancyFilters),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

        public static void ApplyMultiTenancy<TTenantId>(this ModelBuilder modelBuilder, TTenantId tenantId)
        {
            var entityTypeList = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in entityTypeList)
                ConfigureMultiTenancyMethodInfo
                    .MakeGenericMethod(entityType.ClrType, tenantId.GetType())
                    .Invoke(null, new object[] {modelBuilder, entityType, tenantId});
        }

        private static void ConfigureMultiTenancyFilters<TEntity, TTenantId>(ModelBuilder modelBuilder,
            IMutableEntityType entityType, TTenantId tenantId)
            where TEntity : class
        {
            if (ShouldFilterMultiTenancy<TTenantId>(entityType) == false) return;
            var multiTenancyExpression = CreateMultiTenancyExpression<TEntity, TTenantId>(tenantId);
            if (multiTenancyExpression != null)
                modelBuilder.Entity<TEntity>().AddQueryFilter(multiTenancyExpression);
        }

        private static bool ShouldFilterMultiTenancy<TTenantId>(IMutableEntityType entityType)
        {
            return typeof(ITenant<TTenantId>).IsAssignableFrom(entityType.ClrType);
        }

        private static Expression<Func<TEntity, bool>>
            CreateMultiTenancyExpression<TEntity, TTenantId>(TTenantId tenantId)
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> multiTenancyFilter =
                e => ((ITenant<TTenantId>) e).TenantId.Equals(tenantId);
            return multiTenancyFilter;
        }
    }
}