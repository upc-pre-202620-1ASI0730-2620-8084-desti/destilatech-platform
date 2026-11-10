using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DestilatechPlatform.API.Migrations
{
    /// <inheritdoc />
    public partial class AlignIamWithWebapp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_type",
                table: "users",
                newName: "business_type");

            migrationBuilder.RenameColumn(
                name: "preferred_plan",
                table: "users",
                newName: "preferred_plan_code");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "users",
                newName: "full_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "preferred_plan_code",
                table: "users",
                newName: "preferred_plan");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "users",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "business_type",
                table: "users",
                newName: "user_type");
        }
    }
}
