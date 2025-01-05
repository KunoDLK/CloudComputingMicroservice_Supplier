using System;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;

namespace LaMaCo.Comments.Api.Data;

public class ProductDbContext : DbContext
{
      public DbSet<SupplierProduct> SupplierProduct { get; set; } = null!;

      public ProductDbContext(DbContextOptions<ProductDbContext> options)
          : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder builder)
      {
            base.OnModelCreating(builder);

            builder.Entity<SupplierProduct>(x =>
            {
                  x.Property(c => c.OurProductId).IsRequired();
                  x.Property(c => c.SupplierProductId).IsRequired();
                  x.Property(c => c.Supplier).IsRequired();
            });
      }
}



