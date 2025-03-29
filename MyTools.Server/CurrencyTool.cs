using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyTools.Server;

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