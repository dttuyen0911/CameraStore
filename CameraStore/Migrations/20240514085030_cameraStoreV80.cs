using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV80 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Orders_orderID",
                table: "Feedbacks");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Orders_orderID",
                table: "Feedbacks",
                column: "orderID",
                principalTable: "Orders",
                principalColumn: "orderID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Orders_orderID",
                table: "Feedbacks");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Orders_orderID",
                table: "Feedbacks",
                column: "orderID",
                principalTable: "Orders",
                principalColumn: "orderID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
