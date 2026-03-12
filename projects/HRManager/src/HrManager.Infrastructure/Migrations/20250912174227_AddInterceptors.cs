using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInterceptors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "users",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "users",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "users",
                newName: "created_on_utc");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "schedules",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "schedules",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "schedules",
                newName: "created_on_utc");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "positions",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "positions",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "positions",
                newName: "created_on_utc");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "employees",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "employees",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "employees",
                newName: "created_on_utc");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "employee_documents",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "employee_documents",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "employee_documents",
                newName: "created_on_utc");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "employee_absence_balances",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "employee_absence_balances",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "employee_absence_balances",
                newName: "created_on_utc");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "departments",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "departments",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "departments",
                newName: "created_on_utc");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "attendance_records",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "attendance_records",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "attendance_records",
                newName: "created_on_utc");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "hr",
                table: "absence_requests",
                newName: "updated_on_utc");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                schema: "hr",
                table: "absence_requests",
                newName: "deleted_on_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "hr",
                table: "absence_requests",
                newName: "created_on_utc");

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "positions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "positions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "positions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "employees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "employee_documents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "employee_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "employee_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "employee_absence_balances",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "employee_absence_balances",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "employee_absence_balances",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "departments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "attendance_records",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "attendance_records",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "attendance_records",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                schema: "hr",
                table: "absence_requests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by",
                schema: "hr",
                table: "absence_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by",
                schema: "hr",
                table: "absence_requests",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "users");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "schedules");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "employee_absence_balances");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "employee_absence_balances");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "employee_absence_balances");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "attendance_records");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "attendance_records");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "attendance_records");

            migrationBuilder.DropColumn(
                name: "created_by",
                schema: "hr",
                table: "absence_requests");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                schema: "hr",
                table: "absence_requests");

            migrationBuilder.DropColumn(
                name: "updated_by",
                schema: "hr",
                table: "absence_requests");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "users",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "schedules",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "schedules",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "schedules",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "positions",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "positions",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "positions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "employees",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "employees",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "employees",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "employee_documents",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "employee_documents",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "employee_documents",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "employee_absence_balances",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "employee_absence_balances",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "employee_absence_balances",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "departments",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "departments",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "departments",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "attendance_records",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "attendance_records",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "attendance_records",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "updated_on_utc",
                schema: "hr",
                table: "absence_requests",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "deleted_on_utc",
                schema: "hr",
                table: "absence_requests",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "created_on_utc",
                schema: "hr",
                table: "absence_requests",
                newName: "created_at");
        }
    }
}
