using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OvertimeHoursInAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_absence_requests_employees_approver_id",
                schema: "hr",
                table: "absence_requests");

            migrationBuilder.DropColumn(
                name: "approved_at",
                schema: "hr",
                table: "absence_requests");

            migrationBuilder.AddColumn<int>(
                name: "overtime_hours",
                schema: "hr",
                table: "attendance_records",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "reason",
                schema: "hr",
                table: "absence_requests",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "approver_id",
                schema: "hr",
                table: "absence_requests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "processed_at",
                schema: "hr",
                table: "absence_requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_absence_requests_employees_approver_id",
                schema: "hr",
                table: "absence_requests",
                column: "approver_id",
                principalSchema: "hr",
                principalTable: "employees",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_absence_requests_employees_approver_id",
                schema: "hr",
                table: "absence_requests");

            migrationBuilder.DropColumn(
                name: "overtime_hours",
                schema: "hr",
                table: "attendance_records");

            migrationBuilder.DropColumn(
                name: "processed_at",
                schema: "hr",
                table: "absence_requests");

            migrationBuilder.AlterColumn<string>(
                name: "reason",
                schema: "hr",
                table: "absence_requests",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "approver_id",
                schema: "hr",
                table: "absence_requests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "approved_at",
                schema: "hr",
                table: "absence_requests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "fk_absence_requests_employees_approver_id",
                schema: "hr",
                table: "absence_requests",
                column: "approver_id",
                principalSchema: "hr",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
