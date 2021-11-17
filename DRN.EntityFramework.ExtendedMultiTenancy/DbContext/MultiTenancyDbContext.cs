using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.ExtensionUtilities.Entities.Abstract;
using EntityFramework.ExtensionUtilities.Extends;
using EntityFramework.ExtensionUtilities.ModelCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFramework.ExtensionUtilities.DbContext
{
    /// <summary>
    ///     This db context provides multi tenancy , soft delete pattern and audit log on db objects.
    /// </summary>
    /// <typeparam name="TTenantId">TenantId Property Type Of Application</typeparam>
    /// <typeparam name="TUserId">UserId Property Type Of Application</typeparam>
    public class MultiTenancyDbContext<TTenantId, TUserId> : Microsoft.EntityFrameworkCore.DbContext
    {
        internal readonly bool _isAdminContext;
        internal readonly TTenantId TenantId;
        internal readonly TUserId UserId;

        /// <summary>
        /// </summary>
        /// <param name="tenantId">Tenant Id Value Of Current Tenant</param>
        /// <param name="userId"> User Id Value Of Current User</param>
        /// <param name="isAdminContext">
        ///     if you set this parameter to true ; data isolation, AuditLogging and SoftDelete become disabled.
        /// </param>
        public MultiTenancyDbContext(TTenantId tenantId, TUserId userId, bool isAdminContext = false)
        {
            TenantId = tenantId;
            UserId = userId;
            _isAdminContext = isAdminContext;
        }

        public MultiTenancyDbContext()
        {
            _isAdminContext = true;
        }

        public override int SaveChanges()
        {
            SetExtendedProperties();
            return base.SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, ExtendedModelCacheKeyFactory<TTenantId, TUserId>>();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (_isAdminContext == false)
            {
                modelBuilder.ApplySoftDeletePattern(UserId);
                modelBuilder.ApplyMultiTenancy(TenantId);
            }

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetExtendedProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            SetExtendedProperties();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetExtendedProperties();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void SetExtendedProperties()
        {
            if (_isAdminContext == false)
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    switch (entry.State)
                    {
                        case EntityState.Deleted when entry.Entity is ISoftDelete<TUserId> softDeletedEntry:
                            entry.State = EntityState.Modified;
                            softDeletedEntry.IsDeleted = true;
                            softDeletedEntry.DeletedAt = DateTime.Now;
                            softDeletedEntry.DeletedBy = UserId;
                            break;
                        case EntityState.Modified when entry.Entity is IAuditLog<TUserId> modifiedAuditLogEntry:
                            modifiedAuditLogEntry.UpdatedAt = DateTime.Now;
                            modifiedAuditLogEntry.UpdatedBy = UserId;
                            break;
                        case EntityState.Added when entry.Entity is IAuditLog<TUserId> addedAuditLogEntry:
                            addedAuditLogEntry.CreatedAt = DateTime.Now;
                            addedAuditLogEntry.CreatedBy = UserId;
                            break;
                    }

                    if (entry.State == EntityState.Added && entry.Entity is ITenant<TTenantId> tenantEntity)
                        tenantEntity.CustomerId = TenantId;
                }
            }
        }
    }
}