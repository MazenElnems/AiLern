using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR(200)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    AvailableUntil = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    IsPublished = table.Column<bool>(type: "BIT", nullable: false),
                    MaximumAttempts = table.Column<int>(type: "INT", nullable: false),
                    ShowCorrectAnswersAfterClose = table.Column<bool>(type: "BIT", nullable: false),
                    TotalPoints = table.Column<double>(type: "FLOAT", nullable: false),
                    ShuffleQuestions = table.Column<bool>(type: "bit", nullable: false),
                    ShuffleOptions = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionText = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    QuestionType = table.Column<string>(type: "NVARCHAR(20)", nullable: false),
                    Mark = table.Column<double>(type: "FLOAT", nullable: false),
                    Order = table.Column<int>(type: "INT", nullable: false),
                    Instructions = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    QuizId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    OptionNumber = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptionText = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => new { x.OptionNumber, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId",
                table: "Quizzes",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
