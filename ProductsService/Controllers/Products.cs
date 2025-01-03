using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;
using LaMaCo.Comments.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Polly;
using Polly.CircuitBreaker;
using System.Collections.Concurrent;
using NuGet.Common;
using System.Data.Common;

namespace Products_Service.Controllers
{


      [Route("api/[controller]")]
      [ApiController]
      public class Products : ControllerBase
      {
            private const int CacheItemExpiryTimeMinutes = 10;
            private const int MaximumCachedItems = 1000;
            private const int MaximumGetItemCount = 10;
            private readonly ProductDbContext _db;
            private readonly OverwritingCircularQueue<Product> _cache = new(MaximumCachedItems, TimeSpan.FromMinutes(CacheItemExpiryTimeMinutes));
            private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

            public Products(ProductDbContext dbContexts)
            {
                  _db = dbContexts;

                  // Configure Polly Circuit Breaker
                  _circuitBreakerPolicy = Policy
                      .Handle<Exception>() // Handle any exception
                      .CircuitBreakerAsync(
                          exceptionsAllowedBeforeBreaking: 5, // Number of exceptions before breaking the circuit
                          durationOfBreak: TimeSpan.FromSeconds(5), // Duration of circuit break
                          onBreak: (exception, breakDelay) =>
                          {
                                Console.WriteLine($"Circuit broken! Exception: {exception.Message}");
                          },
                          onReset: () =>
                          {
                                Console.WriteLine("Circuit reset!");
                          },
                          onHalfOpen: () =>
                          {
                                Console.WriteLine("Circuit in half-open state.");
                          });
            }

            // GET: api/Products/{searchTerm}
            [HttpGet]
            [Authorize]
            public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string searchTerm)
            {
                  bool circuitBroken = false;
                  List<Product> returnProducts = new();
                  List<Product> DBProducts;
                  List<Product> cachedProducts;

                  // Check if the search term is null or empty
                  if (string.IsNullOrEmpty(searchTerm))
                  {
                        cachedProducts = _cache.GetCachedItems().Take(MaximumGetItemCount).ToList();
                        returnProducts.AddRange(cachedProducts);

                        if (cachedProducts.Count() < MaximumGetItemCount)
                        {
                              try
                              {

                                    await _circuitBreakerPolicy.ExecuteAsync(async () =>
                                    {
                                          // If no search term is provided, return the first "ItemLimit" products
                                          DBProducts = _db.Products.Take(MaximumGetItemCount).ToList();

                                          foreach (var item in DBProducts)
                                          {
                                                _cache.Enqueue(item.Id, item);
                                          }

                                          returnProducts.Clear();
                                          returnProducts.AddRange(DBProducts);
                                    });
                              }
                              catch (BrokenCircuitException ex)
                              {
                                    circuitBroken = true;
                                    // Proceed
                              }
                        }
                  }
                  else
                  {
                        cachedProducts = _cache.GetCachedItems().Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()) || p.Description.ToLower().Contains(searchTerm.ToLower()))
                                                            .Take(MaximumGetItemCount) // Limit the results to the specified item limit
                                                            .ToList();
                        returnProducts.AddRange(cachedProducts);

                        if (cachedProducts.Count() < MaximumGetItemCount)
                        {
                              try
                              {

                                    await _circuitBreakerPolicy.ExecuteAsync(async () =>
                                    {

                                          DBProducts = _db.Products
                                                .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
                                                .Take(MaximumGetItemCount) // Limit the results to the specified item limit
                                                .ToList();

                                          foreach (var item in DBProducts)
                                          {
                                                _cache.Enqueue(item.Id, item);
                                          }

                                          returnProducts.Clear();
                                          returnProducts.AddRange(DBProducts);

                                          if (cachedProducts.Count() < MaximumGetItemCount)
                                          {

                                                DBProducts = _db.Products
                                                      .Where(p => (!(p.Name.ToLower().Contains(searchTerm.ToLower()))) && (p.Description.ToLower().Contains(searchTerm.ToLower())))
                                                      .Take(MaximumGetItemCount - returnProducts.Count()) // Limit the results to the specified item limit
                                                      .ToList();

                                                foreach (var item in DBProducts)
                                                {
                                                      _cache.Enqueue(item.Id, item);
                                                }

                                                returnProducts.AddRange(DBProducts);
                                          }
                                    });
                              }
                              catch (BrokenCircuitException ex)
                              {
                                    circuitBroken = true;
                                    // Proceed
                              }
                        }
                  }

                  if (returnProducts.Count() > 0 && circuitBroken)
                  {
                        return StatusCode(503, "Service is temporarily unavailable. Please try again later.");
                  }
                  else
                  {
                        // Even if database is broken if we got item to return pretend everything is okay
                        // Humans get upset if they have to wait...
                        return Ok(returnProducts);
                  }
            }

            /// GET: api/Products/5
            [HttpGet("{id}")]
            [Authorize]
            public async Task<ActionResult<Product>> GetProductById(int id)
            {
                  bool circuitBroken = false;
                  Product? returnProduct = null;

                  // Check if the product is in the cache
                  returnProduct = _cache.GetCachedItems().FirstOrDefault(x => x.Id == id);

                  if (returnProduct == null)
                  {
                        try
                        {
                              // Attempt to retrieve the product from the database using the circuit breaker policy
                              await _circuitBreakerPolicy.ExecuteAsync(async () =>
                              {
                                    returnProduct = _db.Products.FirstOrDefault(p => p.Id == id);

                                    if (returnProduct != null)
                                    {
                                          // Add the retrieved product to the cache
                                          _cache.Enqueue(returnProduct.Id, returnProduct);
                                    }
                              });
                        }
                        catch (BrokenCircuitException ex)
                        {
                              circuitBroken = true;
                        }
                  }

                  if (returnProduct == null && circuitBroken)
                  {
                        // If the product is not found and the circuit breaker is open, respond with 503
                        return StatusCode(503, "Service is temporarily unavailable. Please try again later.");
                  }
                  else if (returnProduct == null)
                  {
                        // If the product is not found (even after fallback), return 404
                        return NotFound($"Product with ID {id} not found.");
                  }
                  else
                  {
                        // Return the product as a successful response
                        return Ok(returnProduct);
                  }
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

                  var productInDb = _db.Products.FirstOrDefault(p => p.Id == id);
                  if (productInDb == null)
                  {
                        return NotFound();
                  }

                  // Update fields
                  productInDb.Name = product.Name;
                  productInDb.Price = product.Price;

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