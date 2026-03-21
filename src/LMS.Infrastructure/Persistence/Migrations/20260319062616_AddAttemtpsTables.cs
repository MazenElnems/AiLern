using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttemtpsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttemptTimeLimit",
                table: "Quizzes",
                type: "INT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<int>(type: "INT", nullable: false),
                    QuizId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    AttemptEndTime = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    StartAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    SavedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    AttemptNumber = table.Column<int>(type: "INT", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attempts_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attempts_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttemptAnswers",
                columns: table => new
                {
                    AttemptId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    QuestionId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    BooleanAnswer = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    WrittenAnswer = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    OptionNumber = table.Column<int>(type: "INT", nullable: true),
                    Mark = table.Column<double>(type: "FLOAT", nullable: true),
                    Feedback = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptAnswers", x => new { x.AttemptId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "Attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_QuestionId",
                table: "AttemptAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_QuizId",
                table: "Attempts",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_StudentId",
                table: "Attempts",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttemptAnswers");

            migrationBuilder.DropTable(
                name: "Attempts");

            migrationBuilder.DropColumn(
                name: "AttemptTimeLimit",
                table: "Quizzes");
        }
    }
}
