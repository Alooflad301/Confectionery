using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Confectionery.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOrderedToBaskasdad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id_Order",
                table: "Baskets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_Id_Order",
                table: "Baskets",
                column: "Id_Order");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Orders_Id_Order",
                table: "Baskets",
                column: "Id_Order",
                principalTable: "Orders",
                principalColumn: "Id_Order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Orders_Id_Order",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_Id_Order",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "Id_Order",
                table: "Baskets");
        }
    }
}
