using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Confectionery.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOrderedToBaskasdadASASA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderCatalogs",
                columns: table => new
                {
                    Id_OrderCatalog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Order = table.Column<int>(type: "int", nullable: false),
                    Id_Catalog = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCatalogs", x => x.Id_OrderCatalog);
                    table.ForeignKey(
                        name: "FK_OrderCatalogs_Catalog_Id_Catalog",
                        column: x => x.Id_Catalog,
                        principalTable: "Catalog",
                        principalColumn: "Id_Catalog",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderCatalogs_Orders_Id_Order",
                        column: x => x.Id_Order,
                        principalTable: "Orders",
                        principalColumn: "Id_Order",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderCatalogs_Id_Catalog",
                table: "OrderCatalogs",
                column: "Id_Catalog");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCatalogs_Id_Order",
                table: "OrderCatalogs",
                column: "Id_Order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderCatalogs");
        }
    }
}
