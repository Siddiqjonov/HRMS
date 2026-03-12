using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedAttendanceRecordDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_early_department",
                schema: "hr",
                table: "attendance_records",
                newName: "is_early_departure");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_early_departure",
                schema: "hr",
                table: "attendance_records",
                newName: "is_early_department");
        }
    }
}
