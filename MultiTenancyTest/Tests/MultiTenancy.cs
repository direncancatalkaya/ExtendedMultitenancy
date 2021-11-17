using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiTenancyTest.DbContext;
using MultiTenancyTest.Entities;

namespace MultiTenancyTest.Tests
{
    [TestClass]
    public class MultiTenancy : BaseTestClass
    {
        [TestMethod]
        public void MultiTenancy_Global_Filter_Should_Isolate_Other_Tenants_Data()
        {
            var dbContext = new TestDbContext(1, 1, Root, true);
            var products = new List<Product>
            {
                new() {Name = "Product1", CustomerId = 1},
                new() {Name = "Product2", CustomerId = 1},
                new() {Name = "Product3", CustomerId = 2}
            };
            dbContext.Products.AddRange(products);
            dbContext.SaveChanges();

            var dbContext1 = new TestDbContext(1, 1, Root);
            var dbContext2 = new TestDbContext(2, 2, Root);
            var dbContext3 = new TestDbContext(3, 3, Root);

            var countOfTenant1Product = dbContext1.Products.Count();
            var countOfTenant2Product = dbContext2.Products.Count();
            var countOfTenant3Product = dbContext3.Products.Count();

            Assert.AreEqual(countOfTenant1Product, 2);
            Assert.AreEqual(countOfTenant2Product, 1);
            Assert.AreEqual(countOfTenant3Product, 0);
        }

        [TestMethod]
        public void MultiTenancy_AutoWrite_Should_Write_Entity_TenantId()
        {
            var dbContext = new TestDbContext(1, 1);
            var product = new Product {Name = "Product4"};
            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            var addedProduct = dbContext.Products.FirstOrDefault(product => product.Name == "Product4");

            Assert.AreEqual(addedProduct.CustomerId, 1);
        }

        [TestMethod]
        public async Task MultiTenancy_AutoWrite_Should_Write_Entity_TenantId_OnAsync()
        {
            var dbContext = new TestDbContext(1, 1);
            var product = new Product {Name = "Product5"};
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            var addedProduct = dbContext.Products.FirstOrDefault(product => product.Name == "Product5");

            Assert.AreEqual(addedProduct.CustomerId, 1);
        }
    }
}