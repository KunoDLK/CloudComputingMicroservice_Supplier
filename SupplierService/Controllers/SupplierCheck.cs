using Microsoft.AspNetCore.Mvc;
using LaMaCo.Comments.Api.Data;
using Products_Service.Supplier;
using Products_Service.Supplier.Implementation;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;

namespace Products_Service.Controllers
{
    [Route("api/suppliercheck")]
    [ApiController]
    public class SupplierCheckController : ControllerBase
    {
        private readonly SupplierDbContext _db;

        public SupplierCheckController(SupplierDbContext dbContexts)
        {
            _db = dbContexts;
        }

        // GET: api/suppliercheck/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBestSupplierProduct(int id)
        {
            // Find all supplier products for the given OurProductId
            var supplierProducts = await _db.SupplierProducts
                .Where(sp => sp.OurProductId == id)
                .ToListAsync();

            if (!supplierProducts.Any())
            {
                return NotFound($"No products found for OurProductId {id}.");
            }

            SupplierProductInfo bestSupplierProduct = null;

            foreach (var supplierProduct in supplierProducts)
            {
                // Find the corresponding supplier implementation
                var supplier = SupplierList.Suppliers.FirstOrDefault(s => ((int)s.SupplierSource) == supplierProduct.Supplier);
                if (supplier == null)
                {
                    continue; // Skip if supplier is not found
                }

                // Call the supplier's PriceCheck and AvailableStock methods
                var price = supplier.PriceCheck(supplierProduct.Supplier);
                var availableStock = supplier.AvailableStock(supplierProduct.Supplier);

                if (availableStock > 0)
                {
                    if (bestSupplierProduct == null || price < bestSupplierProduct.Price)
                    {
                        bestSupplierProduct = new SupplierProductInfo
                        {
                            Id = supplierProduct.Id,
                            OurProductId = supplierProduct.OurProductId,
                            Supplier = supplierProduct.Supplier,
                            SupplierProductId = supplierProduct.SupplierProductId,
                            Price = price,
                            AvailableStock = availableStock
                        };
                    }
                }
            }

            if (bestSupplierProduct == null)
            {
                return NotFound("No suppliers with stock availability and valid pricing found.");
            }

            return Ok(bestSupplierProduct);
        }
    }
}