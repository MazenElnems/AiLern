using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Course_Image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionNumber",
                table: "Sections");

            migrationBuilder.AddColumn<string>(
                name: "ImageStoragePath",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageStoragePath",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "SectionNumber",
                table: "Sections",
                type: "INT",
                nullable: false,
                defaultValue: 0);
        }
    }
}
