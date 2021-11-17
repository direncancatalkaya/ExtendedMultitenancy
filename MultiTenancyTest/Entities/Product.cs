using System;
using EntityFramework.ExtensionUtilities.Entities.Abstract;

namespace MultiTenancyTest.Entities
{
    public class Product : ITenant<int>, ISoftDelete<int>, IAuditLog<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int CustomerId { get; set; }
    }
}