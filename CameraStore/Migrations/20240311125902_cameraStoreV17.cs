using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "proUrlImage",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "proUrlImages",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "proUrlImages",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "proUrlImage",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
