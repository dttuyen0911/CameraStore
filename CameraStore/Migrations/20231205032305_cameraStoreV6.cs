using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Accounts_accountID",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Accounts_accountID",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Accounts_accountID",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.RenameColumn(
                name: "accountID",
                table: "Orders",
                newName: "customerID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_accountID",
                table: "Orders",
                newName: "IX_Orders_customerID");

            migrationBuilder.RenameColumn(
                name: "accountID",
                table: "Feedbacks",
                newName: "customerID");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_accountID",
                table: "Feedbacks",
                newName: "IX_Feedbacks_customerID");

            migrationBuilder.RenameColumn(
                name: "accountID",
                table: "Carts",
                newName: "customerID");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_accountID",
                table: "Carts",
                newName: "IX_Carts_customerID");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    customerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullname = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    roleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.customerID);
                    table.ForeignKey(
                        name: "FK_Customers_Roles_roleID",
                        column: x => x.roleID,
                        principalTable: "Roles",
                        principalColumn: "roleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_roleID",
                table: "Customers",
                column: "roleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Customers_customerID",
                table: "Carts",
                column: "customerID",
                principalTable: "Customers",
                principalColumn: "customerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Customers_customerID",
                table: "Feedbacks",
                column: "customerID",
                principalTable: "Customers",
                principalColumn: "customerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_customerID",
                table: "Orders",
                column: "customerID",
                principalTable: "Customers",
                principalColumn: "customerID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Customers_customerID",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Customers_customerID",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_customerID",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.RenameColumn(
                name: "customerID",
                table: "Orders",
                newName: "accountID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_customerID",
                table: "Orders",
                newName: "IX_Orders_accountID");

            migrationBuilder.RenameColumn(
                name: "customerID",
                table: "Feedbacks",
                newName: "accountID");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_customerID",
                table: "Feedbacks",
                newName: "IX_Feedbacks_accountID");

            migrationBuilder.RenameColumn(
                name: "customerID",
                table: "Carts",
                newName: "accountID");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_customerID",
                table: "Carts",
                newName: "IX_Carts_accountID");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    accountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleID = table.Column<int>(type: "int", nullable: false),
                    birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fullname = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telephone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.accountID);
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_roleID",
                        column: x => x.roleID,
                        principalTable: "Roles",
                        principalColumn: "roleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_roleID",
                table: "Accounts",
                column: "roleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Accounts_accountID",
                table: "Carts",
                column: "accountID",
                principalTable: "Accounts",
                principalColumn: "accountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Accounts_accountID",
                table: "Feedbacks",
                column: "accountID",
                principalTable: "Accounts",
                principalColumn: "accountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Accounts_accountID",
                table: "Orders",
                column: "accountID",
                principalTable: "Accounts",
                principalColumn: "accountID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
