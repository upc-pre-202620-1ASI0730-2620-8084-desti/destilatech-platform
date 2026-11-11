using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace DestilatechPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsEstimations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "replenishment_estimates",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    has_enough_data = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    average_daily_consumption = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    days_until_threshold = table.Column<int>(type: "int", nullable: true),
                    estimated_date = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    suggested_quantity = table.Column<int>(type: "int", nullable: false),
                    confidence = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    exits_analyzed = table.Column<int>(type: "int", nullable: false),
                    current_quantity_snapshot = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    low_stock_threshold_snapshot = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    calculated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_replenishment_estimates", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_replenishment_estimates_user_id_product_id",
                table: "replenishment_estimates",
                columns: new[] { "user_id", "product_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "replenishment_estimates");
        }
    }
}
