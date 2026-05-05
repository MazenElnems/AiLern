namespace LMS.API.Extensions;

public static class CorsConfigurations
{
    public static IServiceCollection AddCorsConfigurations(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins("http://localhost:5173", "https://ailern.me")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
            );
        });
        return services;
    }
}
