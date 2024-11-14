using System;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;

namespace LaMaCo.Comments.Api.Data;

public class ProductDbContext : DbContext
{
      public DbSet<Product> Products { get; set; } = null!;

      public ProductDbContext(DbContextOptions<ProductDbContext> options)
          : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder builder)
      {
            base.OnModelCreating(builder);

            builder.Entity<Product>(x =>
            {
                  x.Property(c => c.Name).IsRequired();
            });
      }
}



