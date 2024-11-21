using Microsoft.EntityFrameworkCore;
using Products_Service.Controllers;
using Products_Service.Data;
using Xunit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LaMaCo.Comments.Api.Data;

namespace MyApp.Tests
{
    public class ProductsControllerTests
    {
        private ProductDbContext GetDbContext()
        {
            // Generate a unique database name using a GUID
            var uniqueDbName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: uniqueDbName)
                .Options;

            var dbContext = new ProductDbContext(options);

            dbContext.Products.AddRange(new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 10 },
                new Product { Id = 2, Name = "Product 2", Price = 20 },
            });

            dbContext.SaveChanges();

            return dbContext;
        }

        [Fact]
        public void GetProducts_ReturnsAllProducts()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            // Act
            var result = controller.GetProducts();

            // Assert
            Assert.NotNull(result);

            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Value);

            Assert.Equal(2, products.Count());
        }

        [Fact]
        public void GetProductById_ExistingId_ReturnsProduct()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            // Act
            var result = controller.GetProductById(1);

            // Assert
            Assert.NotNull(result.Value);
            Assert.IsType<Product>(result.Value);
            Assert.Equal(1, result.Value.Id);
        }

        [Fact]
        public void GetProductById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            // Act
            var result = controller.GetProductById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}