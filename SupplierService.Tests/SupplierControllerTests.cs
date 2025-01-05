using LaMaCo.Comments.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products_Service.Controllers;
using Products_Service.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class SupplierControllerTests
{
    private SupplierDbContext GetDbContext()
    {
        var uniqueDbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<SupplierDbContext>()
            .UseInMemoryDatabase(databaseName: uniqueDbName)
            .Options;

        var dbContext = new SupplierDbContext(options);

        dbContext.SupplierProducts.AddRange(new List<SupplierProduct>
        {
            new SupplierProduct { Id = 1, OurProductId = 101, Supplier = 0, SupplierProductId = 5001 },
            new SupplierProduct { Id = 2, OurProductId = 102, Supplier = 0, SupplierProductId = 5002 }
        });

        dbContext.SaveChanges();

        return dbContext;
    }

    [Fact]
    public async Task GetProduct_ReturnsOk_WhenProductExists()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);

        // Act
        var result = await controller.GetProduct(101);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var product = Assert.IsType<SupplierProduct>(okResult.Value);
        Assert.Equal(101, product.OurProductId);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);

        // Act
        var result = await controller.GetProduct(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Supplier product with ProductId 999 not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task AddSupplierProduct_ReturnsCreated_WhenValidData()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);
        var newProduct = new SupplierProduct { Id = 3, OurProductId = 103, Supplier = 0, SupplierProductId = 5001};

        // Act
        var result = await controller.AddSupplierProduct(newProduct);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var createdProduct = Assert.IsType<SupplierProduct>(createdResult.Value);
        Assert.Equal(103, createdProduct.OurProductId);

        Assert.Equal(3, await dbContext.SupplierProducts.CountAsync());
    }

    [Fact]
    public async Task AddSupplierProduct_ReturnsBadRequest_WhenDataIsNull()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);

        // Act
        var result = await controller.AddSupplierProduct(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid supplier product data.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateSupplierProduct_ReturnsNoContent_WhenValidUpdate()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);
        var updatedProduct = new SupplierProduct { Id = 1, OurProductId = 105, Supplier = 0, SupplierProductId = 5002 };

        // Act
        var result = await controller.UpdateSupplierProduct(1, updatedProduct);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var productInDb = await dbContext.SupplierProducts.FirstOrDefaultAsync(p => p.Id == 1);
        Assert.NotNull(productInDb);
        Assert.Equal(105, productInDb.OurProductId);
    }

    [Fact]
    public async Task UpdateSupplierProduct_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);
        var updatedProduct = new SupplierProduct { Id = 2, OurProductId = 105, Supplier = 0, SupplierProductId = 5001 };

        // Act
        var result = await controller.UpdateSupplierProduct(1, updatedProduct);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID mismatch.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateSupplierProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);
        var updatedProduct = new SupplierProduct { Id = 999, OurProductId = 105, Supplier = 0, SupplierProductId = 5 };

        // Act
        var result = await controller.UpdateSupplierProduct(999, updatedProduct);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Supplier product with ID 999 not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteSupplierProduct_ReturnsNoContent_WhenValidId()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);

        // Act
        var result = await controller.DeleteSupplierProduct(1);

        // Assert
        Assert.IsType<NoContentResult>(result);

        Assert.Null(await dbContext.SupplierProducts.FirstOrDefaultAsync(p => p.Id == 1));
    }

    [Fact]
    public async Task DeleteSupplierProduct_ReturnsNotFound_WhenInvalidId()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierController(dbContext);

        // Act
        var result = await controller.DeleteSupplierProduct(999);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Supplier product with ID 999 not found.", notFoundResult.Value);
    }
}