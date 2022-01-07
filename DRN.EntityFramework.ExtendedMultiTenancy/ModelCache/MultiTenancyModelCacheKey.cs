using EntityFramework.ExtensionUtilities.DbContext;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFramework.ExtensionUtilities.ModelCache
{
    public class ExtendedModelCacheKey<TTenantId, TUserId> : ModelCacheKey
    {
        private readonly bool _isAdminContext;
        private readonly TTenantId _tenantId;

        public ExtendedModelCacheKey(Microsoft.EntityFrameworkCore.DbContext context) : base(context)
        {
            _tenantId = ((MultiTenancyDbContext<TTenantId, TUserId>) context).TenantId;
            _isAdminContext = ((MultiTenancyDbContext<TTenantId, TUserId>) context)._isAdminContext;
        }

        protected override bool Equals(ModelCacheKey other)
        {
            var result = ((ExtendedModelCacheKey<TTenantId, TUserId>) other)._tenantId.Equals(_tenantId);
            var result2 = ((ExtendedModelCacheKey<TTenantId, TUserId>) other)._isAdminContext.Equals(_isAdminContext);
            return result && result2;
        }
    }

    public sealed class ExtendedModelCacheKeyFactory<TTenantId, TUserId> : IModelCacheKeyFactory
    {
        public ExtendedModelCacheKeyFactory(ModelCacheKeyFactoryDependencies dependencies)
        {
        }

        public  object Create(Microsoft.EntityFrameworkCore.DbContext context)
        {
            return new ExtendedModelCacheKey<TTenantId, TUserId>(context);
        }
        
        public  object Create(Microsoft.EntityFrameworkCore.DbContext context,bool designTime)
        {
            if(designTime) return this;
            return new ExtendedModelCacheKey<TTenantId, TUserId>(context);
        }
    }
}
