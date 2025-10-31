using LMS.API.Middleware;
using LMS.Core.Extensions;
using LMS.Infrastructure.Extensions;
using LMS.Infrastructure.Seeders.Interfaces;
using LMS.Shared.Domain.Entities;
using LMS.Shared.DTOs.Authentication;
using LMS.Shared.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAuthentication();

// swagger & open api
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure Services 
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Core Services
builder.Services.AddRequiredServices(builder.Configuration);

var app = builder.Build();

// Seed initial data
using var scope =  app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<ICourseDataSeeder>();     // seed initial courses
await seeder.SeedAsync();


// Configure the HTTP request pipeline.

app.UseCustomExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>();

app.MapPost("/register-new", async (
    RegisterDto request,
    UserManager<ApplicationUser> userManager) =>
{
    var user = new ApplicationUser
    {
        UserName = request.UserName,
        Email = request.Email,
        FullName = request.FullName,
        CreatedAt = DateTime.UtcNow
    };

    user.CreatedBy = "Mohamed";

    var result = await userManager.CreateAsync(user, request.Password);

    if (!result.Succeeded)
    {
        return Results.BadRequest(new
        {
            errors = result.Errors.Select(e => e.Description)
        });
    }

    return Results.Ok(new UserInfoResponse
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        CreatedAt = user.CreatedAt
    });
}).RequireAuthorization(cfg => cfg.RequireRole(UserRoles.Admin));
app.MapControllers();

app.Run();
