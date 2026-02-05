using LMS.Domain.Constants;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Services;

public class DbInitializer : IDbInitializer
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(AppDbContext context, IWebHostEnvironment env, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, ILogger<DbInitializer> logger)
    {
        _context = context;
        _env = env;
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await ApplyMigrationAsync();
            await SeedRolesAsync();

            if (_env.IsDevelopment())
            {
                await SeedDevelopmentUsers();
            }
            else
            {
                await SeedProductionAdminAccount();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
    private async Task ApplyMigrationAsync()
    {
        if ((await _context.Database.GetPendingMigrationsAsync()).Any())
        {
            await _context.Database.MigrateAsync();
             _logger.LogInformation("Applied pending migrations to the database.");
        }
    }

    public async Task SeedRolesAsync()
    {
        if(!_context.Roles.Any())
        {
            await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Admin));
            await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Instructor));
            await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Student));

            _logger.LogInformation("Seeded default roles to database.");
        }
    }

    private async Task SeedDevelopmentUsers()
    {
        if (!_context.Users.Any())
        {
            var adminEmail = _configuration["DefaultAdmin:email"];
            var adminPassword = _configuration["DefaultAdmin:password"];

            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin User",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(adminUser, adminPassword!);

            await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);


            var instructorEmail = _configuration["DefaultInstructor:email"];
            var instructorPassword = _configuration["DefaultInstructor:password"];

            var instructorUser = new ApplicationUser
            {
                UserName = instructorEmail,
                Email = instructorEmail,
                FullName = "Instructor User",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(instructorUser, instructorPassword!);

            await _userManager.AddToRoleAsync(instructorUser, UserRoles.Instructor);

            var studentEmail = _configuration["DefaultStudent:email"];
            var studentPassword = _configuration["DefaultStudent:password"];

            var studentUser = new ApplicationUser
            {
                UserName = studentEmail,
                Email = studentEmail,
                FullName = "Student User",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(studentUser, studentPassword!);

            await _userManager.AddToRoleAsync(studentUser, UserRoles.Student);

            _logger.LogInformation("Seeded development users to database.");
        }
    }

    private async Task SeedProductionAdminAccount()
    {
        if(!_context.Admins.Any())
        {
            var adminEmail = _configuration["ProductionAdminAccount:email"];
            var adminPassword = _configuration["ProductionAdminAccount:password"];

            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Root Admin",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(adminUser, adminPassword!);
            await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);

            _logger.LogInformation("Seeded production admin account to database.");
        }
    }
}
