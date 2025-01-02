using Microsoft.EntityFrameworkCore;
using Products_Service.Controllers;
using Products_Service.Data;
using Xunit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LaMaCo.Comments.Api.Data;
using System.Linq;

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

        [Fact]
        public void CreateProduct_ValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            var newProduct = new Product { Id = 3, Name = "Product 3", Price = 30 };

            // Act
            var result = controller.CreateProduct(newProduct);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var product = Assert.IsType<Product>(createdResult.Value);

            Assert.Equal("Product 3", product.Name);
            Assert.Equal(30, product.Price);
        }

        [Fact]
        public void UpdateProduct_ValidId_UpdatesProduct()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            var updatedProduct = new Product { Id = 1, Name = "Updated Product 1", Price = 15 };

            // Act
            var result = controller.UpdateProduct(1, updatedProduct);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var productInDb = dbContext.Products.Find(1);
            Assert.NotNull(productInDb);
            Assert.Equal("Updated Product 1", productInDb.Name);
            Assert.Equal(15, productInDb.Price);
        }

        [Fact]
        public void UpdateProduct_NonMatchingId_ReturnsBadRequest()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            var updatedProduct = new Product { Id = 2, Name = "Updated Product", Price = 25 };

            // Act
            var result = controller.UpdateProduct(1, updatedProduct);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void UpdateProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            var updatedProduct = new Product { Id = 999, Name = "Non-Existent Product", Price = 50 };

            // Act
            var result = controller.UpdateProduct(999, updatedProduct);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteProduct_ExistingId_DeletesProduct()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            // Act
            var result = controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var productInDb = dbContext.Products.Find(1);
            Assert.Null(productInDb);
        }

        [Fact]
        public void DeleteProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetDbContext();
            var controller = new Products(dbContext);

            // Act
            var result = controller.DeleteProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}