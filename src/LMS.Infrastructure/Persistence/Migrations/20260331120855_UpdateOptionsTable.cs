using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOptionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionOptions",
                table: "QuestionOptions");

            migrationBuilder.AddColumn<Guid>(
                name: "OptionId",
                table: "QuestionOptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionOptions",
                table: "QuestionOptions",
                columns: new[] { "OptionId", "QuestionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionOptions",
                table: "QuestionOptions");

            migrationBuilder.DropColumn(
                name: "OptionId",
                table: "QuestionOptions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionOptions",
                table: "QuestionOptions",
                columns: new[] { "OptionNumber", "QuestionId" });
        }
    }
}
