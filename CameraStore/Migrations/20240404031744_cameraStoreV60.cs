using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV60 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chatbots",
                columns: table => new
                {
                    ChatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chatName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    chatTelephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isSend = table.Column<bool>(type: "bit", nullable: false),
                    chatTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    customerID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chatbots", x => x.ChatID);
                    table.ForeignKey(
                        name: "FK_Chatbots_Customers_customerID",
                        column: x => x.customerID,
                        principalTable: "Customers",
                        principalColumn: "customerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chatbots_customerID",
                table: "Chatbots",
                column: "customerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chatbots");
        }
    }
}
