using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnswersSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttemptAnswers");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    OptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    OptionNumber = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "NVARCHAR(500)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "BIT", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.OptionId);
                    table.ForeignKey(
                        name: "FK_Options_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    AttemptId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    QuestionId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    WrittenAnswer = table.Column<string>(type: "NVARCHAR(3000)", nullable: true),
                    OptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Mark = table.Column<double>(type: "FLOAT", nullable: true),
                    Feedback = table.Column<string>(type: "NVARCHAR(3000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => new { x.AttemptId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_Answers_Attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "Attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answers_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "OptionId");
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_OptionId",
                table: "Answers",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_QuestionId",
                table: "Options",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.CreateTable(
                name: "AttemptAnswers",
                columns: table => new
                {
                    AttemptId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    QuestionId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    BooleanAnswer = table.Column<string>(type: "VARCHAR(5)", nullable: true),
                    Feedback = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Mark = table.Column<double>(type: "FLOAT", nullable: true),
                    OptionNumber = table.Column<int>(type: "INT", nullable: true),
                    WrittenAnswer = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    OptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCorrect = table.Column<bool>(type: "BIT", nullable: false),
                    OptionNumber = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "NVARCHAR(500)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => new { x.OptionId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_QuestionId",
                table: "AttemptAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");
        }
    }
}
