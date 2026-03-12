using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeleteProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "login",
                schema: "hr",
                table: "employees");

            migrationBuilder.AddColumn<string>(
                name: "salt",
                schema: "hr",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "salt",
                schema: "hr",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "login",
                schema: "hr",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
