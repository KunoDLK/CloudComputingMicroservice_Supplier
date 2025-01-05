using System;
using Microsoft.EntityFrameworkCore;
using Products_Service.Data;

namespace LaMaCo.Comments.Api.Data;

public class SupplierDbContext : DbContext
{
      public DbSet<SupplierProduct> SupplierProducts { get; set; } = null!;

      public SupplierDbContext(DbContextOptions<SupplierDbContext> options)
          : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder builder)
      {
            base.OnModelCreating(builder);

            builder.Entity<SupplierProduct>(x =>
            {
                  x.Property(c => c.Id).IsRequired();
                  x.Property(c => c.OurProductId).IsRequired();
                  x.Property(c => c.SupplierProductId).IsRequired();
                  x.Property(c => c.Supplier).IsRequired();
            });
      }
}



