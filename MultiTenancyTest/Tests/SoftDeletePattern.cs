using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiTenancyTest.DbContext;
using MultiTenancyTest.Entities;

namespace MultiTenancyTest.Tests
{
    [TestClass]
    public class SoftDeletePattern : BaseTestClass
    {
        [TestMethod]
        public void SoftDelete_Pattern_Should_Filter_Deleted_Entities()
        {
            var dbContext = new TestDbContext(1, 1, Root, true);
            var products = new List<Product>
            {
                new Product {Name = "Product1", TenantId = 1, IsDeleted = true},
                new Product {Name = "Product2", TenantId = 1, IsDeleted = true},
                new Product {Name = "Product3", TenantId = 1},
                new Product {Name = "Product4", TenantId = 1},
            };
            dbContext.Products.AddRange(products);
            dbContext.SaveChanges();

            var dbContext1 = new TestDbContext(1, 1, Root);
            var countOfTenant1Product = dbContext1.Products.Count();
            var countOfAllProducts = dbContext1.Products.IgnoreQueryFilters().Count();

            Assert.AreEqual(countOfTenant1Product, 2);
            Assert.AreEqual(countOfAllProducts, 4);
        }

        [TestMethod]
        public void SoftDelete_Pattern_Should_Write_UserId_And_DeleteAt_Properties_And_Update_The_Entity()
        {
            var dbContext = new TestDbContext(1, 1);
            var products = new List<Product>
            {
                new Product {Name = "Product1"},
                new Product {Name = "Product2"}
            };
            dbContext.Products.AddRange(products);
            dbContext.SaveChanges();

            var deletedProduct = dbContext.Products.FirstOrDefault(product => product.Name == "Product1");
            dbContext.Products.Remove(deletedProduct);
            dbContext.SaveChanges();

            var deletedProductForInspect = dbContext.Products.IgnoreQueryFilters()
                .FirstOrDefault(product => product.Name == "Product1");

            Assert.AreEqual(deletedProduct.IsDeleted, true);
            Assert.AreEqual(deletedProduct.DeletedBy, 1);
            Assert.IsTrue(deletedProduct.DeletedAt >= DateTime.UtcNow.AddMinutes(-1));
        }
    }
}