namespace LMS.Infrastructure.Settings;

public class WebhookSettings
{
    public string BaseUrl { get; set; }
    public Dictionary<string,string> Endpoints { get; set; }
}
