using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OvertimeAndTotalHoursToMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "total_hours",
                schema: "hr",
                table: "attendance_records",
                newName: "total_minutes");

            migrationBuilder.RenameColumn(
                name: "overtime_hours",
                schema: "hr",
                table: "attendance_records",
                newName: "overtime_minutes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "total_minutes",
                schema: "hr",
                table: "attendance_records",
                newName: "total_hours");

            migrationBuilder.RenameColumn(
                name: "overtime_minutes",
                schema: "hr",
                table: "attendance_records",
                newName: "overtime_hours");
        }
    }
}
