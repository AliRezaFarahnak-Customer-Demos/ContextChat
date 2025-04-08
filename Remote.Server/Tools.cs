using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace FunctionsSnippetTool;

public class Tools(ILogger<Tools> logger)
{
    [Function(nameof(GetWeather))]
    public async Task<string> GetWeather(
        [McpToolTrigger(nameof(GetWeather), "Gets the weather of a location")] ToolInvocationContext context,
        [McpToolProperty(nameof(longitude), "string", @$"{nameof(longitude)} of location")] string longitude,
        [McpToolProperty(nameof(latitude), "string", @$"{nameof(latitude)} of location")] string latitude)
    {
        logger.BeginScope("GetWeather");
        HttpClient client = new();
        string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,weathercode&timezone=auto";
        string weatherJson = await client.GetStringAsync(url);
        logger.LogInformation("Weather data retrieved: {weatherJson}", weatherJson);
        return weatherJson;
    }

    [Function(nameof(GetCurrentTime))]
    public async Task<string> GetCurrentTime(
        [McpToolTrigger(nameof(GetCurrentTime), "Returns the current time")] ToolInvocationContext context,
        [McpToolProperty(nameof(longitude), "string", @$"{nameof(longitude)} of location")] string longitude,
        [McpToolProperty(nameof(latitude), "string", @$"{nameof(latitude)} of location")] string latitude)
    {
        HttpClient client = new();
        string url = $"https://timeapi.io/api/Time/current/coordinate?latitude={latitude}&longitude={longitude}";
        string timeJson = await client.GetStringAsync(url);
        return timeJson;
    }


    private const string CurrencyApiKey = "fca_live_zofOYVfBOGNlQCi5qglCHnnicPV1yMrACZtGrP8n";
    private const string CurrencyBaseUrl = "https://api.freecurrencyapi.com/v1/latest";

    [Function(nameof(GetExchangeRates))]
    public async Task<string> GetExchangeRates(
        [McpToolTrigger(nameof(GetExchangeRates), "Get latest currency exchange rates")] ToolInvocationContext context,
        [McpToolProperty(nameof(fromCurrency), "string", "Base currency code (e.g., USD, EUR)")] string fromCurrency,
        [McpToolProperty(nameof(toCurrencies), "string", "Comma-separated list of currency codes to retrieve (e.g., EUR,USD,CAD)")] string toCurrencies)
    {
        logger.BeginScope("GetExchangeRates");
        HttpClient client = new();
        string url = $"{CurrencyBaseUrl}?apikey={CurrencyApiKey}&base_currency={fromCurrency}&currencies={toCurrencies}";
        string exchangeRatesJson = await client.GetStringAsync(url);
        logger.LogInformation("Exchange rate data retrieved: {exchangeRatesJson}", exchangeRatesJson);
        return exchangeRatesJson;
    }
}
