namespace LMS.API.Extensions;

public static class CorsConfigurations
{
    public static IServiceCollection AddCorsConfigurations(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        return services;
    }
}
