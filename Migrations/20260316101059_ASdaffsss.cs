using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Confectionery.Migrations
{
    /// <inheritdoc />
    public partial class ASdaffsss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BasketCatalogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BasketCatalogs");
        }
    }
}
