using System;
using EntityFramework.ExtensionUtilities.Entities.Abstract;

namespace MultiTenancyTest.Entities
{
    public class Product : ITenant<int>,ISoftDelete<int>,IAuditLog<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}