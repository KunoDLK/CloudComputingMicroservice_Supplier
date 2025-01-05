using LaMaCo.Comments.Api.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products_Service.Controllers;
using Products_Service.Data;

public class SupplierCheckControllerTests
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
    public void GetBestSupplierProduct_ReturnsNotFound_WhenNoProductsExist()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierCheckController(dbContext);

        // Act
        var result = controller.GetBestSupplierProduct(999).Result; // Non-existent product

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void GetBestSupplierProduct_ReturnsData_WhenProductsExist()
    {
        // Arrange
        var dbContext = GetDbContext();
        var controller = new SupplierCheckController(dbContext);

        // Act
        IActionResult result = controller.GetBestSupplierProduct(101).Result;

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.NotNull(okResult.Value);
    }
}