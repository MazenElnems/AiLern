using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeRuleForWeakTopicsRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeakTopics_Attempts_AttemptId",
                table: "WeakTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_WeakTopics_Courses_CourseId",
                table: "WeakTopics");

            migrationBuilder.AddForeignKey(
                name: "FK_WeakTopics_Attempts_AttemptId",
                table: "WeakTopics",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WeakTopics_Courses_CourseId",
                table: "WeakTopics",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeakTopics_Attempts_AttemptId",
                table: "WeakTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_WeakTopics_Courses_CourseId",
                table: "WeakTopics");

            migrationBuilder.RenameColumn(
                name: "Criterion",
                table: "AIGradingCriteria",
                newName: "Criteria");

            migrationBuilder.AddForeignKey(
                name: "FK_WeakTopics_Attempts_AttemptId",
                table: "WeakTopics",
                column: "AttemptId",
                principalTable: "Attempts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeakTopics_Courses_CourseId",
                table: "WeakTopics",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
