using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsManagerOfDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_manager_of_department",
                schema: "hr",
                table: "employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_manager_of_department",
                schema: "hr",
                table: "employees",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
