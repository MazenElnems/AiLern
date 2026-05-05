using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAIStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AIResources",
                newName: "UploadStatus");

            migrationBuilder.AddColumn<int>(
                name: "AIStatus",
                table: "AIResources",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIStatus",
                table: "AIResources");

            migrationBuilder.RenameColumn(
                name: "UploadStatus",
                table: "AIResources",
                newName: "Status");
        }
    }
}
