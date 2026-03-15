using Hangfire;
using Hangfire.Common;
using LMS.API.Extensions;
using LMS.API.Middleware;
using LMS.Application;
using LMS.Infrastructure;
using LMS.Infrastructure.Jobs;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddAuthentication();
builder.Services
    .AddCorsConfigurations()
    .AddSwaggerConfigrations()
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration);

var app = builder.Build();

await app.InitializeDatabaseAsync();

using var scope = app.Services.CreateScope();

// IRecurringJobManager
var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate(
    "remove-expired-refresh-tokens",
    Job.FromExpression<RemoveExpiredRefreshTokensJob>(x => x.ExecuteAsync()),
    Cron.Daily(2)
);

// Request Pipline

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.Run();