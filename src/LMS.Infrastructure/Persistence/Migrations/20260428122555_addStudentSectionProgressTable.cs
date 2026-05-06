using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addStudentSectionProgressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Percent",
                table: "StudentCourseProgress");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StudentCourseProgress");

            migrationBuilder.RenameColumn(
                name: "LastLearningItemId",
                table: "StudentCourseProgress",
                newName: "LastOpenedFileId");

            migrationBuilder.CreateTable(
                name: "StudentSectionProgress",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSectionProgress", x => new { x.StudentId, x.SectionId });
                    table.ForeignKey(
                        name: "FK_StudentSectionProgress_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSectionProgress_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSectionProgress_SectionId",
                table: "StudentSectionProgress",
                column: "SectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSectionProgress");

            migrationBuilder.RenameColumn(
                name: "LastOpenedFileId",
                table: "StudentCourseProgress",
                newName: "LastLearningItemId");

            migrationBuilder.AddColumn<double>(
                name: "Percent",
                table: "StudentCourseProgress",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StudentCourseProgress",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
