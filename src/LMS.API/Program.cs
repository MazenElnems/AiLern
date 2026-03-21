using Hangfire;
using Hangfire.Common;
using LMS.API.Extensions;
using LMS.API.Middleware;
using LMS.Application;
using LMS.Infrastructure;
using LMS.Infrastructure.Jobs;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Formatting.Compact;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext();
});

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddAuthentication();

builder.Services
    .ConfigureHttpClient()
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
    app.UseHangfireDashboard("/hangfire");
}

app.UseSerilogRequestLogging();

app.UseHsts();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();