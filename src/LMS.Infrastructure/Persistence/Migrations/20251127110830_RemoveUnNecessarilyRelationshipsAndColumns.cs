using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnNecessarilyRelationshipsAndColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_Approvedby",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Courses_SectionCourseId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Approvedby",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_SectionCourseId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Approvedby",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "SectionCourseId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "AdminLevel",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "CourseStatus",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CourseStatus",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Pending");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Courses",
                type: "DATETIME2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Approvedby",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SectionCourseId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdminLevel",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Approvedby",
                table: "Courses",
                column: "Approvedby");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SectionCourseId",
                table: "Courses",
                column: "SectionCourseId",
                unique: true,
                filter: "[SectionCourseId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_Approvedby",
                table: "Courses",
                column: "Approvedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Courses_SectionCourseId",
                table: "Courses",
                column: "SectionCourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
