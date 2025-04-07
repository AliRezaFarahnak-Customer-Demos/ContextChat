using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace MyTools.Server.Tools;

[McpServerToolType]
public static class WeatherTool
{
    [McpServerTool, Description("Get weather forecast for a location.")]
    public static async Task<string> GetForecast(
        [Description("Latitude of the location.")] string latitude,
        [Description("Longitude of the location.")] string longitude)
    {
        HttpClient client = new();
        string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,weathercode&timezone=auto";
        string weatherJson = await client.GetStringAsync(url);
        return weatherJson;
    }
}
