using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employees_users_user_id",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropTable(
                name: "users",
                schema: "hr");

            migrationBuilder.DropIndex(
                name: "ix_employees_user_id",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "hr",
                table: "employees");

            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "hr",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "keycloak_user_id",
                schema: "hr",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_employees_email",
                schema: "hr",
                table: "employees",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employees_keycloak_user_id",
                schema: "hr",
                table: "employees",
                column: "keycloak_user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_employees_email",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_employees_keycloak_user_id",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "keycloak_user_id",
                schema: "hr",
                table: "employees");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "hr",
                table: "employees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "users",
                schema: "hr",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    email = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    salt = table.Column<string>(type: "text", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employees_user_id",
                schema: "hr",
                table: "employees",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                schema: "hr",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_users_user_id",
                schema: "hr",
                table: "employees",
                column: "user_id",
                principalSchema: "hr",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
