using System.Linq;
using System.Runtime.Serialization.DataContracts;
using LaMaCo.Comments.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;

namespace Products_Service.Controllers
{
      [Route("api/[controller]")]
      [ApiController]
      public class Products : ControllerBase
      {
            private readonly ProductDbContext _db;

            public Products(ProductDbContext dbContexts)
            {
                  _db = dbContexts;
                  _db.Database.EnsureCreated();
                  _db.Database.Migrate();
            }

            [HttpGet]
            public IEnumerable<Product> GetProducts()
            {
                  return _db.Products.ToList();
            }
      }
}
