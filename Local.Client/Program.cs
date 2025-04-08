using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using ModelContextProtocol.SemanticKernel.Extensions;
using Microsoft.Extensions.Configuration;
class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

        var builder = Kernel.CreateBuilder();
        builder.Services
            .AddLogging()
            .AddAzureOpenAIChatCompletion(
                config["MODEL"],
                config["ENDPOINT"],
                config["API_KEY"]);

        var kernel = builder.Build();

        string serverPath = Path.GetFullPath("../../../../Local.Server/Local.Server.csproj", AppDomain.CurrentDomain.BaseDirectory);

        var transportOptions = new Dictionary<string, string>
        {
            ["command"] = "dotnet",
            ["arguments"] = $"run --project {serverPath} --no-build"
        };
        await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("Local.Server", transportOptions);

        var execution = new OpenAIPromptExecutionSettings
        {
            Temperature = 0,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var history = new ChatHistory();
        var chat = kernel.GetRequiredService<IChatCompletionService>();

        while (true)
        {
            Console.Write("You: ");
            Console.ResetColor();
            var input = Console.ReadLine();

            history.AddUserMessage(input);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Assistant: ");

            var resp = "";
            await foreach (var content in chat.GetStreamingChatMessageContentsAsync(history, execution, kernel))
            {
                Console.Write(content.Content);
                resp += content.Content;
            }
            Console.ResetColor();

            history.AddAssistantMessage(resp);
            Console.WriteLine("\n");
        }
    }
}
