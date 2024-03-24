using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV35 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Carts_cartID",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Products_proID",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Customers_customerID",
                table: "Carts");

            migrationBuilder.AddColumn<bool>(
                name: "isSelect",
                table: "CartDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Carts_cartID",
                table: "CartDetails",
                column: "cartID",
                principalTable: "Carts",
                principalColumn: "cartID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Products_proID",
                table: "CartDetails",
                column: "proID",
                principalTable: "Products",
                principalColumn: "proID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Customers_customerID",
                table: "Carts",
                column: "customerID",
                principalTable: "Customers",
                principalColumn: "customerID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Carts_cartID",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Products_proID",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Customers_customerID",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "isSelect",
                table: "CartDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Carts_cartID",
                table: "CartDetails",
                column: "cartID",
                principalTable: "Carts",
                principalColumn: "cartID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Products_proID",
                table: "CartDetails",
                column: "proID",
                principalTable: "Products",
                principalColumn: "proID");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Customers_customerID",
                table: "Carts",
                column: "customerID",
                principalTable: "Customers",
                principalColumn: "customerID");
        }
    }
}
