using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDeleteBehaviorForFK_AssignmentSubmissions_AspNetUsers_StudentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmissions_AspNetUsers_StudentId",
                table: "AssignmentSubmissions");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_AspNetUsers_StudentId",
                table: "AssignmentSubmissions",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentSubmissions_AspNetUsers_StudentId",
                table: "AssignmentSubmissions");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentSubmissions_AspNetUsers_StudentId",
                table: "AssignmentSubmissions",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
