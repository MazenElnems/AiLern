using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInstructionsAndExplannationColumnsToBeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "NVARCHAR(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Questions",
                type: "NVARCHAR(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "Questions",
                type: "NVARCHAR(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "NVARCHAR(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Questions",
                type: "NVARCHAR(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "Questions",
                type: "NVARCHAR(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)",
                oldNullable: true);
        }
    }
}
