using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Products_proID",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_proID",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "proID",
                table: "Carts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "proID",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_proID",
                table: "Carts",
                column: "proID");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Products_proID",
                table: "Carts",
                column: "proID",
                principalTable: "Products",
                principalColumn: "proID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
