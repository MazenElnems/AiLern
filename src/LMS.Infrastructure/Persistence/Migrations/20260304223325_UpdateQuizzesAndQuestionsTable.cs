using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizzesAndQuestionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuestionType",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "ShowCorrectAnswersAfterClose",
                table: "Quizzes",
                newName: "ShowResultOnClose");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "NVARCHAR(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Quizzes",
                type: "VARCHAR(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "Questions",
                type: "NVARCHAR(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(1500)");

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Questions",
                type: "NVARCHAR(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(1500)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "Questions",
                type: "NVARCHAR(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Questions",
                type: "VARCHAR(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "QuestionOptions",
                type: "NVARCHAR(500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(1500)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "ShowResultOnClose",
                table: "Quizzes",
                newName: "ShowCorrectAnswersAfterClose");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "NVARCHAR(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Quizzes",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "TotalPoints",
                table: "Quizzes",
                type: "FLOAT",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "Questions",
                type: "NVARCHAR(1500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Questions",
                type: "NVARCHAR(1500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(2000)");

            migrationBuilder.AddColumn<string>(
                name: "QuestionType",
                table: "Questions",
                type: "NVARCHAR(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "QuestionOptions",
                type: "NVARCHAR(1500)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(500)");
        }
    }
}
