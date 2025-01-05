using Microsoft.AspNetCore.Mvc;
using LaMaCo.Comments.Api.Data;
using Products_Service.Supplier;
using Products_Service.Supplier.Implementation;

namespace Products_Service.Controllers
{
    [Route("api/Supplier")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ProductDbContext _db;

        private List<IProductSupplier> Suppliers { get; }

        public SupplierController(ProductDbContext dbContexts)
        {
            _db = dbContexts;

            Suppliers = new List<IProductSupplier>
            {
                 new MockProductSupplier(SupplierSources.TestMockSuppliers)
            };
        }

        // GET: api/Supplier
        [HttpGet]
        public IActionResult GetProduct(int ProductId)
        {
            return Ok();
        }
    }
}