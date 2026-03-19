using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusFromBothCoursesAndEnrollmets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CourseStatus",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "RequestedAt",
                table: "Enrollments",
                newName: "EnrolledAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnrolledAt",
                table: "Enrollments",
                newName: "RequestedAt");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<string>(
                name: "CourseStatus",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");
        }
    }
}
