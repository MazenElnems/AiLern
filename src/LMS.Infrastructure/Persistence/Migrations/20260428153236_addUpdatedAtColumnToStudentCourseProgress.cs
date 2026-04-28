using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addUpdatedAtColumnToStudentCourseProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "StudentCourseProgress");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StudentCourseProgress",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StudentCourseProgress");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "StudentCourseProgress",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
