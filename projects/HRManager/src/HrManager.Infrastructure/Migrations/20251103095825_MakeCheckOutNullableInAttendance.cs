using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeCheckOutNullableInAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "check_out",
                schema: "hr",
                table: "attendance_records",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "check_out",
                schema: "hr",
                table: "attendance_records",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone",
                oldNullable: true);
        }
    }
}
