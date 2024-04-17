using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV72 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contentChatbots");

            migrationBuilder.DropTable(
                name: "Chatbots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chatbots",
                columns: table => new
                {
                    ChatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chatName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    chatTelephone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chatbots", x => x.ChatID);
                });

            migrationBuilder.CreateTable(
                name: "contentChatbots",
                columns: table => new
                {
                    chatID = table.Column<int>(type: "int", nullable: false),
                    chatTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isSend = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contentChatbots", x => x.chatID);
                    table.ForeignKey(
                        name: "FK_contentChatbots_Chatbots_chatID",
                        column: x => x.chatID,
                        principalTable: "Chatbots",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
