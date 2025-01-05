﻿// <auto-generated />
using LaMaCo.Comments.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SupplierService.Migrations
{
    [DbContext(typeof(ProductDbContext))]
    [Migration("20250105133103_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("Products_Service.Data.SupplierProduct", b =>
                {
                    b.Property<int>("SupplierProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("OurProductId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Supplier")
                        .HasColumnType("INTEGER");

                    b.HasKey("SupplierProductId");

                    b.ToTable("SupplierProduct");
                });
#pragma warning restore 612, 618
        }
    }
}
