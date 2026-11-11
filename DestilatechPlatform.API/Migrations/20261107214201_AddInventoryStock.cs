using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace DestilatechPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    presentation = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    unit = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: false),
                    category = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    sku = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_products", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "stock_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    current_quantity = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    low_stock_threshold = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_stock_items", x => x.id);
                    table.ForeignKey(
                        name: "f_k_stock_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "stock_movements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    stock_item_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false),
                    reason = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    notes = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    reference = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    movement_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_stock_movements", x => x.id);
                    table.ForeignKey(
                        name: "f_k_stock_movements_stock_items_stock_item_id",
                        column: x => x.stock_item_id,
                        principalTable: "stock_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_products_user_id",
                table: "products",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_stock_items_product_id",
                table: "stock_items",
                column: "product_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_stock_items_user_id",
                table: "stock_items",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_stock_movements_stock_item_id",
                table: "stock_movements",
                column: "stock_item_id");

            migrationBuilder.CreateIndex(
                name: "i_x_stock_movements_user_id_movement_date",
                table: "stock_movements",
                columns: new[] { "user_id", "movement_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_movements");

            migrationBuilder.DropTable(
                name: "stock_items");

            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
