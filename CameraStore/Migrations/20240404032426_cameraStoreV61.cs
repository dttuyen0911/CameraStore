using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV61 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chatbots_Customers_customerID",
                table: "Chatbots");

            migrationBuilder.DropIndex(
                name: "IX_Chatbots_customerID",
                table: "Chatbots");

            migrationBuilder.DropColumn(
                name: "customerID",
                table: "Chatbots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "customerID",
                table: "Chatbots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Chatbots_customerID",
                table: "Chatbots",
                column: "customerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chatbots_Customers_customerID",
                table: "Chatbots",
                column: "customerID",
                principalTable: "Customers",
                principalColumn: "customerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
