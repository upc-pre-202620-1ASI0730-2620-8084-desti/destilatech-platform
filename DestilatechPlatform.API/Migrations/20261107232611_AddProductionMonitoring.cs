using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace DestilatechPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionMonitoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "production_batches",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    variety = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    estimated_quantity = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    tank = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    stage = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    bottled_units = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_production_batches", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "batch_stage_changes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    stage = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    changed_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    notes = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    batch_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_batch_stage_changes", x => x.id);
                    table.ForeignKey(
                        name: "f_k_batch_stage_changes_production_batches_batch_id",
                        column: x => x.batch_id,
                        principalTable: "production_batches",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "process_variables",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    batch_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    min_range = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    max_range = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    sensor_id = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_process_variables", x => x.id);
                    table.ForeignKey(
                        name: "f_k_process_variables__production_batch_batch_id",
                        column: x => x.batch_id,
                        principalTable: "production_batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sensor_readings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    process_variable_id = table.Column<int>(type: "int", nullable: false),
                    batch_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    value = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    source_key = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    within_range = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sensor_readings", x => x.id);
                    table.ForeignKey(
                        name: "f_k_sensor_readings_process_variables_process_variable_id",
                        column: x => x.process_variable_id,
                        principalTable: "process_variables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_batch_stage_changes_batch_id",
                table: "batch_stage_changes",
                column: "batch_id");

            migrationBuilder.CreateIndex(
                name: "i_x_process_variables_batch_id_sensor_id",
                table: "process_variables",
                columns: new[] { "batch_id", "sensor_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_process_variables_batch_id_type",
                table: "process_variables",
                columns: new[] { "batch_id", "type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_process_variables_user_id",
                table: "process_variables",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_production_batches_user_id_code",
                table: "production_batches",
                columns: new[] { "user_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_process_variable_id_source_key",
                table: "sensor_readings",
                columns: new[] { "process_variable_id", "source_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_user_id_recorded_at",
                table: "sensor_readings",
                columns: new[] { "user_id", "recorded_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "batch_stage_changes");

            migrationBuilder.DropTable(
                name: "sensor_readings");

            migrationBuilder.DropTable(
                name: "process_variables");

            migrationBuilder.DropTable(
                name: "production_batches");
        }
    }
}
