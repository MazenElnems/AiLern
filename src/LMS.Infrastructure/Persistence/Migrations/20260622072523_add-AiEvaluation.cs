using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAiEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confidence",
                table: "Answers");

            migrationBuilder.AddColumn<bool>(
                name: "IsRelated",
                table: "Questions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccuracyRating",
                table: "Answers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvaluateComment",
                table: "Answers",
                type: "NVARCHAR(3000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeedbackThemes",
                table: "Answers",
                type: "VARCHAR(40)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRelated",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AccuracyRating",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "EvaluateComment",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "FeedbackThemes",
                table: "Answers");

            migrationBuilder.AddColumn<double>(
                name: "Confidence",
                table: "Answers",
                type: "float",
                nullable: true);
        }
    }
}
