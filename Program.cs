using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using ModelContextProtocol.Client;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Create an MCPClient for the GitHub server
        await using var mcpClient = await McpClientFactory.CreateAsync(
            new()
            {
                Id = "github",
                Name = "GitHub",
                TransportType = "stdio",
                TransportOptions = new Dictionary<string, string>
                {
                    ["command"] = "npx",
                    ["arguments"] = "-y @modelcontextprotocol/server-github",
                }
            },
            new() { ClientInfo = new() { Name = "GitHub", Version = "1.0.0" } }).ConfigureAwait(false);

        // Retrieve the list of tools available on the GitHub server
        var tools = await mcpClient.GetAIFunctionsAsync().ConfigureAwait(false);
        foreach (var tool in tools)
        {
            Console.WriteLine($"{tool.Name}: {tool.Description}");
        }

        // Prepare and build kernel with the MCP tools as Kernel functions
        var builder = Kernel.CreateBuilder();
        builder.Services
            .AddLogging()
            .AddAzureOpenAIChatCompletion(
                deploymentName: Environment.GetEnvironmentVariable("MODEL"),
                endpoint: Environment.GetEnvironmentVariable("ENDPOINT"),
                apiKey: Environment.GetEnvironmentVariable("API_KEY"));
        Kernel kernel = builder.Build();
        kernel.Plugins.AddFromFunctions("GitHub", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

        // Enable automatic function calling
        OpenAIPromptExecutionSettings executionSettings = new()
        {
            Temperature = 0,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true })
        };

        // Test using GitHub tools
        var prompt = "Summarize the last four commits to the microsoft/semantic-kernel repository in a very funny way and with emojis";
        Console.WriteLine($"\n\n{prompt}\n");

        // Create a chat history
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(prompt);

        // Get the chat completion service
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Stream the response
        string completeResponse = "";
        await foreach (var content in chatCompletionService.GetStreamingChatMessageContentsAsync(
            chatHistory, executionSettings, kernel))
        {
            Console.Write(content.Content);
            completeResponse += content.Content;
        }

        // Add the response to the chat history
        chatHistory.AddAssistantMessage(completeResponse);
        Console.WriteLine();
    }
}
