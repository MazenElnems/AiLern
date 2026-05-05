using LMS.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace LMS.API.Extensions;

public static class HttpClientConfiguration
{
    public static IServiceCollection ConfigureHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient("AIService", (serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AIServiceSettings>>().Value;

            httpClient.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}
