using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using System.IO;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Dynamically find the server project file
        string serverProjectPath = FindServerProjectFile("MyTools.Server");

        // Create an MCPClient for the MyTools server
        await using var MyToolsClient = await McpClientFactory.CreateAsync(
            new()
            {
                Id = "MyTools",
                Name = "MyTools",
                TransportType = TransportTypes.StdIo,
                TransportOptions = new Dictionary<string, string>
                {
                    ["command"] = "dotnet",
                    ["arguments"] = $"run --project {serverProjectPath}",
                }
            },
            new() { ClientInfo = new() { Name = "MyTools", Version = "1.0.0" } }).ConfigureAwait(false);

        // Retrieve the list of tools available on the MyTools server
        var MyToolsTools = await MyToolsClient.GetAIFunctionsAsync().ConfigureAwait(false);

        // Display available MyTools tools with color
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Available MyTools Tools:");
        Console.ResetColor();

        foreach (var tool in MyToolsTools)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  {tool.Name}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(tool.Description);
        }
        Console.ResetColor();

        // Create an MCPClient for the GitHub server
       /* await using var githubClient = await McpClientFactory.CreateAsync(
            new()
            {
                Id = "github",
                Name = "GitHub",
                TransportType = TransportTypes.StdIo,
                TransportOptions = new Dictionary<string, string>
                {
                    ["command"] = "npx",
                    ["arguments"] = "-y @modelcontextprotocol/server-github",
                }
            },
            new() { ClientInfo = new() { Name = "GitHub", Version = "1.0.0" } }).ConfigureAwait(false);

        // Retrieve the list of tools available on the GitHub server
        var githubTools = await githubClient.GetAIFunctionsAsync().ConfigureAwait(false);

        // Display available tools with color
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Available GitHub Tools:");
        Console.ResetColor();

        foreach (var tool in githubTools)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  {tool.Name}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(tool.Description);
        }
        Console.ResetColor();*/

        // Prepare and build kernel with the MCP tools as Kernel functions
        var builder = Kernel.CreateBuilder();
        builder.Services
            .AddLogging()
            .AddAzureOpenAIChatCompletion(
                deploymentName: Environment.GetEnvironmentVariable("MODEL"),
                endpoint: Environment.GetEnvironmentVariable("ENDPOINT"),
                apiKey: Environment.GetEnvironmentVariable("API_KEY"));
        Kernel kernel = builder.Build();

        // Add both tools to the kernel
        kernel.Plugins.AddFromFunctions("MyTools", MyToolsTools.Select(aiFunction => aiFunction.AsKernelFunction()));
        kernel.Plugins.AddFromFunctions("GitHub", githubTools.Select(aiFunction => aiFunction.AsKernelFunction()));

        // Enable automatic function calling
        OpenAIPromptExecutionSettings executionSettings = new()
        {
            Temperature = 0,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
        };

        // Create a chat history
        var chatHistory = new ChatHistory();

        // Get the chat completion service
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Welcome message
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\n🚀 Welcome to Context Chat Assistant 🚀");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Type 'exit' to quit or 'clear' to reset the conversation.");
        Console.ResetColor();
        Console.WriteLine();

        // Main chat loop
        while (true)
        {
            // Get user input
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("You: ");
            Console.ResetColor();
            var userInput = Console.ReadLine();

            // Check for exit command
            if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Goodbye!");
                Console.ResetColor();
                break;
            }

            // Check for clear command
            if (string.Equals(userInput, "clear", StringComparison.OrdinalIgnoreCase))
            {
                chatHistory.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Conversation cleared.");
                Console.ResetColor();
                Console.WriteLine();
                continue;
            }

            // Add user message to history
            chatHistory.AddUserMessage(userInput);

            // Display assistant response
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Assistant: ");
            Console.ForegroundColor = ConsoleColor.Cyan;

            // Stream the response
            string completeResponse = "";
            await foreach (var content in chatCompletionService.GetStreamingChatMessageContentsAsync(
                chatHistory, executionSettings, kernel))
            {
                Console.Write(content.Content);
                completeResponse += content.Content;
            }
            Console.ResetColor();

            // Add the response to the chat history
            chatHistory.AddAssistantMessage(completeResponse);
            Console.WriteLine("\n");
        }
    }

    // Helper method to find the server project file
    private static string FindServerProjectFile(string serverName)
    {
        // Get the current application directory
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        // Navigate to the solution root directory (assuming standard project structure)
        // Go up to bin directory
        string solutionDirectory = Path.GetFullPath(Path.Combine(currentDirectory, "..\\..\\..\\.."));

        // Look for the server project in the solution directory
        var serverProjectFile = Directory.GetFiles(solutionDirectory, "*.csproj", SearchOption.AllDirectories)
            .FirstOrDefault(file => file.Contains(serverName, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(serverProjectFile))
        {
            throw new FileNotFoundException($"Could not find server project file for {serverName}");
        }

        Console.WriteLine($"Starting dotnet MCP server: {serverProjectFile}");
        return serverProjectFile;
    }
}
