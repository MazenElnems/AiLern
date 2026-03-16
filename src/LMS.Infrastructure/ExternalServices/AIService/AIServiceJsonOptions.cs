using System.Text.Json;
using System.Text.Json.Serialization;

namespace LMS.Infrastructure.ExternalServices.AIService;

internal static class AIServiceJsonOptions
{
    internal static JsonSerializerOptions Default =>
        new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

}
