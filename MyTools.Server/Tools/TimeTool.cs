using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyTools.Server.Tools;

[McpServerToolType]
public static class TimeTool
{
    private const string ApiBaseUrl = "https://timeapi.io/api/Time/current/coordinate";

    [McpServerTool, Description("Get current time data for a location.")]
    public static async Task<string> GetCurrentTime(
        [Description("Latitude of the location.")] string latitude,
        [Description("Longitude of the location.")] string longitude)
    {
        HttpClient client = new();
        string url = $"{ApiBaseUrl}?latitude={latitude}&longitude={longitude}";
        string timeJson = await client.GetStringAsync(url);
        return timeJson;
    }
}