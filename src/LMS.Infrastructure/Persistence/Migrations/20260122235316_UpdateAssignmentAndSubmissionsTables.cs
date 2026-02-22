using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssignmentAndSubmissionsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignmentSubmissionFiles",
                table: "AssignmentSubmissionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignmentFiles",
                table: "AssignmentFiles");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "AssignmentSubmissionFiles");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "AssignmentFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AssignmentSubmissionFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "AssignmentSubmissionFiles",
                type: "NVARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UploadStatus",
                table: "AssignmentSubmissionFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AssignmentFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "AssignmentFiles",
                type: "NVARCHAR(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UploadStatus",
                table: "AssignmentFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignmentSubmissionFiles",
                table: "AssignmentSubmissionFiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignmentFiles",
                table: "AssignmentFiles",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignmentSubmissionFiles",
                table: "AssignmentSubmissionFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignmentFiles",
                table: "AssignmentFiles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AssignmentSubmissionFiles");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "AssignmentSubmissionFiles");

            migrationBuilder.DropColumn(
                name: "UploadStatus",
                table: "AssignmentSubmissionFiles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AssignmentFiles");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "AssignmentFiles");

            migrationBuilder.DropColumn(
                name: "UploadStatus",
                table: "AssignmentFiles");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "AssignmentSubmissionFiles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "AssignmentFiles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignmentSubmissionFiles",
                table: "AssignmentSubmissionFiles",
                column: "FileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignmentFiles",
                table: "AssignmentFiles",
                column: "FileId");
        }
    }
}
