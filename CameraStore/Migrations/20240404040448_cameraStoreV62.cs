using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV62 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "chatTime",
                table: "Chatbots");

            migrationBuilder.DropColumn(
                name: "content",
                table: "Chatbots");

            migrationBuilder.DropColumn(
                name: "isSend",
                table: "Chatbots");

            migrationBuilder.CreateTable(
                name: "contentChatbots",
                columns: table => new
                {
                    chatID = table.Column<int>(type: "int", nullable: false),
                    isSend = table.Column<bool>(type: "bit", nullable: false),
                    chatTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contentChatbots");

            migrationBuilder.AddColumn<DateTime>(
                name: "chatTime",
                table: "Chatbots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "Chatbots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isSend",
                table: "Chatbots",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
