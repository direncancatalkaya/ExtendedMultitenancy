using EntityFramework.ExtensionUtilities.DbContext;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFramework.ExtensionUtilities.ModelCache
{
    public class ExtendedModelCacheKey<TTenantId, TUserId> : ModelCacheKey where TUserId : struct
    {
        private readonly TTenantId _tenantId;
        private readonly bool _isAdminContext;

        public ExtendedModelCacheKey(Microsoft.EntityFrameworkCore.DbContext context) : base(context)
        {
            _tenantId = ((MultiTenancyDbContext<TTenantId, TUserId>) context).TenantId;
            _isAdminContext = ((MultiTenancyDbContext<TTenantId, TUserId>) context)._isAdminContext;
        }

        protected override bool Equals(ModelCacheKey other)
        {
            bool result = ((ExtendedModelCacheKey<TTenantId, TUserId>) other)._tenantId.Equals(_tenantId);
            bool result2 = ((ExtendedModelCacheKey<TTenantId, TUserId>) other)._isAdminContext.Equals(_isAdminContext);
            return (result && result2);
        }
    }

    public sealed class ExtendedModelCacheKeyFactory<TTenantId, TUserId> : ModelCacheKeyFactory
        where TUserId : struct
    {
        public override object Create(Microsoft.EntityFrameworkCore.DbContext context)
        {
            return new ExtendedModelCacheKey<TTenantId, TUserId>(context);
        }

        public ExtendedModelCacheKeyFactory(ModelCacheKeyFactoryDependencies dependencies) : base(dependencies)
        {
        }
    }
}