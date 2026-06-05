using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAIGradingCriteriaColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAIGradingEnabled",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AIGradingReferenceAnswer",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeakTopics",
                table: "Attempts",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Assignments",
                type: "NVARCHAR(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)");

            migrationBuilder.CreateTable(
                name: "AIGradingCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mark = table.Column<double>(type: "float", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIGradingCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIGradingCriteria_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIGradingCriteria_QuestionId",
                table: "AIGradingCriteria",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIGradingCriteria");

            migrationBuilder.DropColumn(
                name: "IsAIGradingEnabled",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "AIGradingReferenceAnswer",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "WeakTopics",
                table: "Attempts");

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Assignments",
                type: "NVARCHAR(MAX)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(MAX)",
                oldNullable: true);
        }
    }
}
