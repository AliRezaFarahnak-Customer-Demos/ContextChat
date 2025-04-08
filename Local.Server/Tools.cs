using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Local.Server.Tools;

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

[McpServerToolType]
public static class TimeTool
{
    [McpServerTool, Description("Get current time data for a location.")]
    public static async Task<string> GetCurrentTime(
        [Description("Latitude of the location.")] string latitude,
        [Description("Longitude of the location.")] string longitude)
    {
        HttpClient client = new();
        string url = $"https://timeapi.io/api/Time/current/coordinate?latitude={latitude}&longitude={longitude}";
        string timeJson = await client.GetStringAsync(url);
        return timeJson;
    }
}


[McpServerToolType]
public static class CurrencyTool
{
    private const string ApiKey = "fca_live_zofOYVfBOGNlQCi5qglCHnnicPV1yMrACZtGrP8n";
    private const string BaseUrl = "https://api.freecurrencyapi.com/v1/latest";

    [McpServerTool, Description("Get latest currency exchange rates.")]
    public static async Task<string> GetExchangeRates(
        [Description("Base currency code (e.g., USD, EUR)")] string fromCurrency,
        [Description("Comma-separated list of currency codes to retrieve (e.g., EUR,USD,CAD)")] string toCurrencies)
    {
        HttpClient client = new();
        string url = $"{BaseUrl}?apikey={ApiKey}&base_currency={fromCurrency}&currencies={toCurrencies}";
        string exchangeRatesJson = await client.GetStringAsync(url);
        return exchangeRatesJson;
    }
}


