using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace DestilatechPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertsNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    source_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    severity = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    episode_key = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    attended_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_alerts", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_alerts_account_id",
                table: "alerts",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "i_x_alerts_episode_key",
                table: "alerts",
                column: "episode_key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");
        }
    }
}
