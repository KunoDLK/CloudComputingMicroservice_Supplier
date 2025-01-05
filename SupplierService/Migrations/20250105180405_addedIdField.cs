using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplierService.Migrations
{
    /// <inheritdoc />
    public partial class addedIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SupplierProduct",
                table: "SupplierProduct");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierProductId",
                table: "SupplierProduct",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SupplierProduct",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupplierProduct",
                table: "SupplierProduct",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SupplierProduct",
                table: "SupplierProduct");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SupplierProduct");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierProductId",
                table: "SupplierProduct",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupplierProduct",
                table: "SupplierProduct",
                column: "SupplierProductId");
        }
    }
}
