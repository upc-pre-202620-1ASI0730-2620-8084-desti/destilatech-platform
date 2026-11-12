using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DestilatechPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTimestampsToInventoryAndProduction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "stock_movements",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "stock_movements",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "stock_items",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "stock_items",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "sensor_readings",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "sensor_readings",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "products",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "products",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "production_batches",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "production_batches",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "process_variables",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "process_variables",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "stock_items");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "stock_items");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "products");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "products");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "production_batches");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "production_batches");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "process_variables");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "process_variables");
        }
    }
}
