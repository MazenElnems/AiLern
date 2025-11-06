using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            var concurrencyStamp1 = Guid.NewGuid().ToString();
            var concurrencyStamp2 = Guid.NewGuid().ToString();
            var concurrencyStamp3 = Guid.NewGuid().ToString();

            migrationBuilder.Sql(@$"
                INSERT INTO AspNetRoles([Name],[NormalizedName],[ConcurrencyStamp])
                VALUES 
                ('Admin','ADMIN','{concurrencyStamp1}'),
                ('Instructor','INSTRUCTOR','{concurrencyStamp2}'),
                ('Student','STUDENT','{concurrencyStamp3}')
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetRoles");
        }
    }
}
