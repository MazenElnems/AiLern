using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJsonColumnsToAnswersAndAttemptsShuffledQuestionIdsAndOptionIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShuffledQuestionIds",
                table: "Attempts",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShuffledOptionIds",
                table: "Answers",
                type: "NVARCHAR(MAX)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShuffledQuestionIds",
                table: "Attempts");

            migrationBuilder.DropColumn(
                name: "ShuffledOptionIds",
                table: "Answers");
        }
    }
}
