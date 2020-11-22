using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiTenancyTest.DbContext;
using MultiTenancyTest.Entities;

namespace MultiTenancyTest.Tests
{
    [TestClass]
    public class AuditLog : BaseTestClass
    {
        [TestMethod]
        public void Audit_Log_Should_Write_CreatedAt_ModifiedAt_CreatedBy_ModifiedBy_Properties()
        {
            var dbContext = new TestDbContext(3, 2);

            var addedProduct = new Product {Name = "Product10"};
            dbContext.Products.Add((addedProduct));
            dbContext.SaveChanges();

            var updatedProduct = dbContext.Products.FirstOrDefault(product => product.Name == "Product10");
            updatedProduct.Name = "Product20";
            dbContext.Products.Update(updatedProduct);
            dbContext.SaveChanges();

            var productForInspect = dbContext.Products.FirstOrDefault(product => product.Name == "Product20");

            Assert.AreEqual(productForInspect.CreatedBy, 2);
            Assert.AreEqual(productForInspect.ModifiedBy, 2);
            Assert.IsTrue(productForInspect.CreatedAt >= DateTime.UtcNow.AddMinutes(-1));
            Assert.IsTrue(productForInspect.ModifiedAt >= DateTime.UtcNow.AddMinutes(-1));
        }
    }
}