using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV75 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "orderID",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "orderID1",
                table: "Feedbacks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_orderID",
                table: "Feedbacks",
                column: "orderID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_orderID1",
                table: "Feedbacks",
                column: "orderID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Orders_orderID",
                table: "Feedbacks",
                column: "orderID",
                principalTable: "Orders",
                principalColumn: "orderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Orders_orderID1",
                table: "Feedbacks",
                column: "orderID1",
                principalTable: "Orders",
                principalColumn: "orderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Orders_orderID",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Orders_orderID1",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_orderID",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_orderID1",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "orderID",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "orderID1",
                table: "Feedbacks");
        }
    }
}
