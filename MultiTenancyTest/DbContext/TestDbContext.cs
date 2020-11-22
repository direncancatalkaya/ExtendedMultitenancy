using System;
using System.Collections.Generic;
using EntityFramework.ExtensionUtilities.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MultiTenancyTest.Entities;

namespace MultiTenancyTest.DbContext
{
    public class TestDbContext : MultiTenancyDbContext<int, int>
    {
        private InMemoryDatabaseRoot _dbRoot;

        public TestDbContext(int tenantId, int userId, InMemoryDatabaseRoot dbRoot = null,
            bool isAdminContext = false) : base(
            tenantId, userId,
            isAdminContext)
        {
            _dbRoot = dbRoot;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_dbRoot != null) optionsBuilder.UseInMemoryDatabase("Test", _dbRoot);
            else optionsBuilder.UseInMemoryDatabase("Test");
            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<Product> Products { get; set; }
    }
}