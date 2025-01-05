using Microsoft.AspNetCore.Mvc;
using LaMaCo.Comments.Api.Data;
using Products_Service.Supplier;
using Products_Service.Supplier.Implementation;

namespace Products_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase 
    {
        private readonly ProductDbContext _db;

        private Dictionary<SupplierSources, IProductSupplier> Suppliers { get; set; }

        public SupplierController(ProductDbContext dbContexts)
        {
            _db = dbContexts;

            Suppliers = new Dictionary<SupplierSources, IProductSupplier>
            {
                { SupplierSources.TestMockSuppliers, new MockProductSupplier() } 
            };
        }

        // GET: api/Supplier
        [HttpGet]
        public IActionResult GetProducts() 
        {
            return Ok();
        }
    }
}