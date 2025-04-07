using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using ModelContextProtocol.SemanticKernel.Extensions;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

class Program
{
    private static Process _serverProcess;

    static async Task Main(string[] args)
    {
        // Set up cleanup on application exit
        AppDomain.CurrentDomain.ProcessExit += (s, e) => CleanupServerProcess();
        Console.CancelKeyPress += (s, e) => CleanupServerProcess();

        var cfg = new ConfigurationBuilder().AddUserSecrets<Program>().AddEnvironmentVariables().Build();

        var k = Kernel.CreateBuilder();
        k.Services.AddLogging().AddAzureOpenAIChatCompletion(cfg["MODEL"], cfg["ENDPOINT"], cfg["API_KEY"]);
        var kernel = k.Build();

        string serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../MyTools.Server/MyTools.Server.csproj");
        serverPath = Path.GetFullPath(serverPath);

        // Start the server process manually
        _serverProcess = StartServerProcess(serverPath);

        // Configure transport to use the already started process
        var transportOptions = new Dictionary<string, string>
        {
            ["command"] = "dotnet",
            ["arguments"] = $"run --project \"{serverPath}\" --no-build"
        };
        await kernel.Plugins.AddMcpFunctionsFromStdioServerAsync("MyTools", transportOptions);

        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var history = new ChatHistory();
        var chat = kernel.GetRequiredService<IChatCompletionService>();

        try
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("You: ");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input) || input.ToLower() == "exit")
                {
                    break;
                }

                history.AddUserMessage(input);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Assistant: ");

                var resp = "";
                await foreach (var content in chat.GetStreamingChatMessageContentsAsync(history, settings, kernel))
                {
                    Console.Write(content.Content);
                    resp += content.Content;
                }
                Console.ResetColor();

                history.AddAssistantMessage(resp);
                Console.WriteLine("\n");
            }
        }
        finally
        {
            CleanupServerProcess();
        }
    }

    private static Process StartServerProcess(string serverPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{serverPath}\" --no-build",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };
        
        process.Start();
        return process;
    }

    private static void CleanupServerProcess()
    {
        if (_serverProcess != null && !_serverProcess.HasExited)
        {
            try
            {
                _serverProcess.Kill(entireProcessTree: true);
                _serverProcess.Dispose();
                _serverProcess = null;
                Console.WriteLine("Server process terminated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error terminating server process: {ex.Message}");
            }
        }
    }
}
