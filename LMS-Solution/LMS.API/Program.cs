using LMS.API.Middleware;
using LMS.Core.Extensions;
using LMS.Infrastructure.Extensions;
using LMS.Infrastructure.Seeders.Interfaces;

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
var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();     // seed initial courses
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

app.MapControllers();

app.Run();
