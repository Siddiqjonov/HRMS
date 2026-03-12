using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveKeykclaokIdFromEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_employees_keycloak_user_id",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "keycloak_user_id",
                schema: "hr",
                table: "employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "keycloak_user_id",
                schema: "hr",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_employees_keycloak_user_id",
                schema: "hr",
                table: "employees",
                column: "keycloak_user_id",
                unique: true);
        }
    }
}
