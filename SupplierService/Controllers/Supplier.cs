using Microsoft.AspNetCore.Mvc;
using LaMaCo.Comments.Api.Data;
using Products_Service.Supplier;
using Products_Service.Supplier.Implementation;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;

namespace Products_Service.Controllers
{
    public static class SupplierList
    {
        public static List<ProductSupplier> Suppliers = new List<ProductSupplier>
        {
            new MockProductSupplier(SupplierSources.TestMockSuppliers)
        };
    }

    [Route("api/Supplier")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ProductDbContext _db;

        public SupplierController(ProductDbContext dbContexts)
        {
            _db = dbContexts;
        }

        // GET: api/Supplier
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var supplierProduct = await _db.SupplierProducts.FirstOrDefaultAsync(sp => sp.OurProductId == productId);
            if (supplierProduct == null)
            {
                return NotFound($"Supplier product with ProductId {productId} not found.");
            }

            return Ok(supplierProduct);
        }

        // POST: api/Supplier
        [HttpPost]
        public async Task<IActionResult> AddSupplierProduct([FromBody] SupplierProduct supplierProduct)
        {
            if (supplierProduct == null)
            {
                return BadRequest("Invalid supplier product data.");
            }

            await _db.SupplierProducts.AddAsync(supplierProduct);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { productId = supplierProduct.OurProductId }, supplierProduct);
        }

        // PUT: api/Supplier/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplierProduct(int id, [FromBody] SupplierProduct updatedSupplierProduct)
        {
            if (id != updatedSupplierProduct.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var existingSupplierProduct = await _db.SupplierProducts.FirstOrDefaultAsync(sp => sp.Id == id);
            if (existingSupplierProduct == null)
            {
                return NotFound($"Supplier product with ID {id} not found.");
            }

            existingSupplierProduct.OurProductId = updatedSupplierProduct.OurProductId;
            existingSupplierProduct.Supplier = updatedSupplierProduct.Supplier;
            existingSupplierProduct.SupplierProductId = updatedSupplierProduct.SupplierProductId;

            _db.SupplierProducts.Update(existingSupplierProduct);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Supplier/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplierProduct(int id)
        {
            var supplierProduct = await _db.SupplierProducts.FirstOrDefaultAsync(sp => sp.Id == id);
            if (supplierProduct == null)
            {
                return NotFound($"Supplier product with ID {id} not found.");
            }

            _db.SupplierProducts.Remove(supplierProduct);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}