using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEnableAIGradingColumnAndAddConfidenceColumnToAnswersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAIGradingEnabled",
                table: "Quizzes");

            migrationBuilder.AddColumn<double>(
                name: "Confidence",
                table: "Answers",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confidence",
                table: "Answers");

            migrationBuilder.AddColumn<bool>(
                name: "IsAIGradingEnabled",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
