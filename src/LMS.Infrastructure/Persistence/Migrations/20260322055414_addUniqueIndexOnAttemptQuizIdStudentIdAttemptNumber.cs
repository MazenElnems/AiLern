using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addUniqueIndexOnAttemptQuizIdStudentIdAttemptNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Attempts_QuizId_StudentId_AttemptNumber",
                table: "Attempts",
                columns: new[] { "QuizId", "StudentId", "AttemptNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attempts_QuizId_StudentId_AttemptNumber",
                table: "Attempts");
        }
    }
}
