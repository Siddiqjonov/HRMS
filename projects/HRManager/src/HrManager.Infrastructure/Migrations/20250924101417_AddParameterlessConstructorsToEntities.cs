using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParameterlessConstructorsToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "emplooyee_id",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.RenameColumn(
                name: "date_of_bitrth",
                schema: "hr",
                table: "employees",
                newName: "date_of_birth");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "termination_date",
                schema: "hr",
                table: "employees",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date_of_birth",
                schema: "hr",
                table: "employees",
                newName: "date_of_bitrth");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "termination_date",
                schema: "hr",
                table: "employees",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "emplooyee_id",
                schema: "hr",
                table: "employee_documents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
