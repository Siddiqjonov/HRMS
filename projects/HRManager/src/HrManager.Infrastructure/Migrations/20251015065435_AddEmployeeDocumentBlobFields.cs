using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeDocumentBlobFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "file_size",
                schema: "hr",
                table: "employee_documents",
                newName: "file_size_in_bytes");

            migrationBuilder.AlterColumn<string>(
                name: "file_path",
                schema: "hr",
                table: "employee_documents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                schema: "hr",
                table: "employee_documents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "blob_name",
                schema: "hr",
                table: "employee_documents",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "blob_url",
                schema: "hr",
                table: "employee_documents",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "container_name",
                schema: "hr",
                table: "employee_documents",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "content_type",
                schema: "hr",
                table: "employee_documents",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "document_type",
                schema: "hr",
                table: "employee_documents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "uploaded_by_user_id",
                schema: "hr",
                table: "employee_documents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blob_name",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.DropColumn(
                name: "blob_url",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.DropColumn(
                name: "container_name",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.DropColumn(
                name: "content_type",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.DropColumn(
                name: "document_type",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.DropColumn(
                name: "uploaded_by_user_id",
                schema: "hr",
                table: "employee_documents");

            migrationBuilder.RenameColumn(
                name: "file_size_in_bytes",
                schema: "hr",
                table: "employee_documents",
                newName: "file_size");

            migrationBuilder.AlterColumn<string>(
                name: "file_path",
                schema: "hr",
                table: "employee_documents",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                schema: "hr",
                table: "employee_documents",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }
    }
}
