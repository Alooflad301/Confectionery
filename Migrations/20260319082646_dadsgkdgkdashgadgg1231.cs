using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Confectionery.Migrations
{
    /// <inheritdoc />
    public partial class dadsgkdgkdashgadgg1231 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Catalog",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Catalog");
        }
    }
}
