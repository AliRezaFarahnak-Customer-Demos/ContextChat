# ContextChat

A .NET solution demonstrating Model Context Protocol (MCP) for AI-powered chat applications with external tools.

## Project Structure
- **ContextChat.Client**: Console app connecting to MCP servers using Azure OpenAI
- **QuickstartWeatherServer**: MCP server providing weather forecast functionality

## Prerequisites
- .NET SDK 9.0 (or 8.0 for Weather Server)
- Node.js and npm
- Azure OpenAI API key and endpoint

## Setup

1. **Clone the Repository**
```bash
git clone [repository-url]
cd ContextChat
```

2. **Configure Environment Variables**
```bash
cd ContextChat.Client
dotnet user-secrets set "MODEL" "your-azure-openai-deployment-name"
dotnet user-secrets set "ENDPOINT" "your-azure-openai-endpoint"
dotnet user-secrets set "API_KEY" "your-azure-openai-api-key"
```

## Running the Application

### Weather Server
- Standard: `cd QuickstartWeatherServer && dotnet run`
- MCP Inspector: `cd QuickstartWeatherServer && npx @modelcontextprotocol/inspector dotnet run`

### Client
- In a separate terminal: `cd ContextChat.Client && dotnet run`
- Connects automatically to GitHub MCP server and Azure OpenAI

## Usage
- Ask questions related to GitHub or weather
- Commands: `clear` (reset conversation), `exit` (quit)

## How It Works
The client connects to MCP servers and integrates them with Azure OpenAI via Semantic Kernel. The AI model determines which tool to use based on your query.

## Development
- Add new tools to `WeatherTools.cs` with `[McpServerTool]` and `[Description]` attributes
- Use MCP Inspector for debugging protocol communication