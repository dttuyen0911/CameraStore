using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Customers_customerID1",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_customerID1",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "customerID1",
                table: "Carts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "customerID1",
                table: "Carts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_customerID1",
                table: "Carts",
                column: "customerID1",
                unique: true,
                filter: "[customerID1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Customers_customerID1",
                table: "Carts",
                column: "customerID1",
                principalTable: "Customers",
                principalColumn: "customerID");
        }
    }
}
