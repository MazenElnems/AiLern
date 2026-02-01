using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MaterialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR(200)", nullable: false),
                    SectionNumber = table.Column<int>(type: "INT", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "NVARCHAR(255)", nullable: false),
                    UploadStatus = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    OrderIndex = table.Column<int>(type: "INT", nullable: false),
                    StoragePath = table.Column<string>(type: "NVARCHAR(500)", nullable: false),
                    FileType = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    FileSize = table.Column<long>(type: "BIGINT", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialFiles_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialFiles_SectionId",
                table: "MaterialFiles",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId",
                table: "Sections",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialFiles");

            migrationBuilder.DropTable(
                name: "Sections");
        }
    }
}
