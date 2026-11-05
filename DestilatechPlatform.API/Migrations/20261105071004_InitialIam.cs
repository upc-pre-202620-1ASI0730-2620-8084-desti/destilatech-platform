using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace DestilatechPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialIam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false),
                    business_name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    user_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    preferred_plan = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    trial_started_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    trial_ends_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    access_granted_until = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
