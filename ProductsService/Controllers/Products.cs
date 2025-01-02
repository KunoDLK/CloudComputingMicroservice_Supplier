using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;
using LaMaCo.Comments.Api.Data;
using Microsoft.AspNetCore.Authorization;

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
            }

            // GET: api/Products
            [HttpGet]
            [Authorize]
            public ActionResult<IEnumerable<Product>> GetProducts()
            {
                  return _db.Products.ToList();
            }

            // GET: api/Products/5
            [HttpGet("{id}")]
            [Authorize]
            public ActionResult<Product> GetProductById(int id)
            {
                  var product = _db.Products.Find(id);
                  if (product == null)
                  {
                        return NotFound();
                  }
                  return product;
            }

            // POST: api/Products
            [HttpPost]
            [Authorize]
            public ActionResult<Product> CreateProduct(Product product)
            {
                  _db.Products.Add(product);
                  _db.SaveChanges();

                  return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }

            // PUT: api/Products/5
            [HttpPut("{id}")]
            [Authorize]
            public IActionResult UpdateProduct(int id, Product product)
            {
                  if (id != product.Id)
                  {
                        return BadRequest();
                  }

                  _db.Entry(product).State = EntityState.Modified;
                  try
                  {
                        _db.SaveChanges();
                  }
                  catch (DbUpdateConcurrencyException)
                  {
                        if (!_db.Products.Any(e => e.Id == id))
                        {
                              return NotFound();
                        }
                        else
                        {
                              throw;
                        }
                  }

                  return NoContent();
            }

            // DELETE: api/Products/5
            [HttpDelete("{id}")]
            [Authorize]
            public IActionResult DeleteProduct(int id)
            {
                  var product = _db.Products.Find(id);
                  if (product == null)
                  {
                        return NotFound();
                  }

                  _db.Products.Remove(product);
                  _db.SaveChanges();

                  return NoContent();
            }
      }
}