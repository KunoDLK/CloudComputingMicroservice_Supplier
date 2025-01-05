using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplierService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierProduct",
                columns: table => new
                {
                    SupplierProductId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OurProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Supplier = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProduct", x => x.SupplierProductId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierProduct");
        }
    }
}
