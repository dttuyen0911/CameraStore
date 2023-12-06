using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraStore.Migrations
{
    /// <inheritdoc />
    public partial class cameraStoreV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    cateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cateName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    cateDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    cateImage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.cateID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    roleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.roleID);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    supID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    supName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    supDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    supAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    supTelephone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.supID);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    accountID = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_Accounts", x => x.accountID);
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_roleID",
                        column: x => x.roleID,
                        principalTable: "Roles",
                        principalColumn: "roleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    proID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    proName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    proDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    proImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    proDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    proQuantity = table.Column<int>(type: "int", nullable: true),
                    proSale = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    proStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    proPercent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    proPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    supID = table.Column<int>(type: "int", nullable: false),
                    cateID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.proID);
                    table.ForeignKey(
                        name: "FK_Products_Categories_cateID",
                        column: x => x.cateID,
                        principalTable: "Categories",
                        principalColumn: "cateID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_supID",
                        column: x => x.supID,
                        principalTable: "Suppliers",
                        principalColumn: "supID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    orderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    orderDelivery = table.Column<DateTime>(type: "datetime2", nullable: false),
                    orderStatus = table.Column<bool>(type: "bit", nullable: false),
                    orderPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    orderAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    paymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    totalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    accountID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.orderID);
                    table.ForeignKey(
                        name: "FK_Orders_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    cartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountID = table.Column<int>(type: "int", nullable: false),
                    proID = table.Column<int>(type: "int", nullable: false),
                    cartQuantityTotal = table.Column<int>(type: "int", nullable: false),
                    cartPriceTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    timeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.cartID);
                    table.ForeignKey(
                        name: "FK_Carts_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carts_Products_proID",
                        column: x => x.proID,
                        principalTable: "Products",
                        principalColumn: "proID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    feedID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountID = table.Column<int>(type: "int", nullable: false),
                    proID = table.Column<int>(type: "int", nullable: false),
                    feedDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.feedID);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Accounts_accountID",
                        column: x => x.accountID,
                        principalTable: "Accounts",
                        principalColumn: "accountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Products_proID",
                        column: x => x.proID,
                        principalTable: "Products",
                        principalColumn: "proID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    orderID = table.Column<int>(type: "int", nullable: false),
                    proID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => new { x.orderID, x.proID });
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_orderID",
                        column: x => x.orderID,
                        principalTable: "Orders",
                        principalColumn: "orderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_proID",
                        column: x => x.proID,
                        principalTable: "Products",
                        principalColumn: "proID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartDetails",
                columns: table => new
                {
                    cartID = table.Column<int>(type: "int", nullable: false),
                    proID = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDetails", x => new { x.cartID, x.proID });
                    table.ForeignKey(
                        name: "FK_CartDetails_Carts_cartID",
                        column: x => x.cartID,
                        principalTable: "Carts",
                        principalColumn: "cartID");
                    table.ForeignKey(
                        name: "FK_CartDetails_Products_proID",
                        column: x => x.proID,
                        principalTable: "Products",
                        principalColumn: "proID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_roleID",
                table: "Accounts",
                column: "roleID");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_proID",
                table: "CartDetails",
                column: "proID");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_accountID",
                table: "Carts",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_proID",
                table: "Carts",
                column: "proID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_accountID",
                table: "Feedbacks",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_proID",
                table: "Feedbacks",
                column: "proID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_proID",
                table: "OrderDetails",
                column: "proID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_accountID",
                table: "Orders",
                column: "accountID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_cateID",
                table: "Products",
                column: "cateID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_supID",
                table: "Products",
                column: "supID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartDetails");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
