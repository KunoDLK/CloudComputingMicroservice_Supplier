using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;
using LaMaCo.Comments.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Polly;
using Polly.CircuitBreaker;
using System.Collections.Concurrent;

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
            private readonly ConcurrentDictionary<int, (Product Product, DateTime CachedTime)> _cache = new();
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
            public async ActionResult<IEnumerable<Product>> GetProducts(string searchTerm)
            {
                  IEnumerable<Product> returnProducts;

                  // Check if the search term is null or empty
                  if (string.IsNullOrEmpty(searchTerm))
                  {
                        returnProducts = (IEnumerable<Product>)_cache.Take(MaximumGetItemCount).ToList();

                        if (returnProducts.Count() < 100)
                        {
                              await _circuitBreakerPolicy.ExecuteAsync(async () =>
                              {
                                    // If no search term is provided, return the first "ItemLimit" products
                                    returnProducts = _db.Products.Take(MaximumGetItemCount).ToList();
                              });
                        }

                        if (returnProducts.Count() > 0)
                        {
                              return Ok(returnProducts);
                        }
                        else
                        {
                              if (_circuitBreakerPolicy.CircuitState != CircuitState.Open)
                              {
                                    return StatusCode(503, "Service is temporarily unavailable. Please try again later.");
                              }
                        }
                  }
                  else
                  {
                        await _circuitBreakerPolicy.ExecuteAsync(async () =>
                        {
                              // Convert both the search term and product name to lowercase for case-insensitive comparison
                              IEnumerable<Product> filteredProducts = _db.Products
                                    .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
                                    .Take(100) // Limit the results to the specified item limit
                                    .ToList();
                        });

                        if (filteredProducts.Count < 100)
                        {
                              await _circuitBreakerPolicy.ExecuteAsync(async () =>
                              {
                                    filteredProducts.AddRange(_db.Products
                                          .Where(p => p.Description.ToLower().Contains(searchTerm.ToLower()))
                                          .Take(100 - filteredProducts.Count) // Limit the results to the specified item limit
                                          .ToList());
                              });
                        }

                        return Ok(filteredProducts);
                  }
            }

            /// GET: api/Products/5
            [HttpGet("{id}")]
            [Authorize]
            public ActionResult<Product> GetProductById(int id)
            {
                  // Check if the product is present in the cache
                  if (_cache.TryGetValue(id, out var cachedEntry))
                  {
                        // Check if the cached entry is still valid (e.g., not expired)
                        var cacheExpirationTime = TimeSpan.FromMinutes(CacheItemExpiryTimeMinutes); // Set cache expiration time as needed
                        if (DateTime.UtcNow - cachedEntry.CachedTime <= cacheExpirationTime)
                        {
                              return cachedEntry.Product; // Return product from cache
                        }
                        else
                        {
                              // If cached data is expired, remove it from the cache
                              _cache.TryRemove(id, out _);
                        }
                  }

                  // If not in cache or if cache is expired, query the database
                  var product = _db.Products.Find(id);
                  if (product == null)
                  {
                        return NotFound();
                  }

                  // Add the product to the cache with the current timestamp
                  _cache[id] = (product, DateTime.UtcNow);

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