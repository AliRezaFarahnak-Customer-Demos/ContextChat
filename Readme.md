# ContextChat

A .NET solution demonstrating Model Context Protocol (MCP) for AI-powered chat applications with external tools.

## Project Structure
- **ContextChat.Client**: Console app connecting to MCP servers using Azure OpenAI
- **MyTools.Server**: MCP server providing weather forecast functionality

## Prerequisites
- .NET SDK 9.0
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
- MCP Inspector: `npx @modelcontextprotocol/inspector dotnet run`

### Client
- In a separate terminal: `cd ContextChat.Client && dotnet run`
- Connects automatically to GitHub MCP server and Azure OpenAI

## How It Works
The client connects to MCP servers and integrates them with Azure OpenAI via Semantic Kernel. The AI model determines which tool to use based on your query.

# Playwright MCP Integration

## Installation in VS Code

To install the Playwright MCP server in VS Code, use the following command:

```bash
code --add-mcp '{"name":"playwright","command":"npx","args":["@playwright/mcp@latest"]}'
```

After installation, restart VS Code. The Playwright MCP tool will be available for use with GitHub Copilot Chat.

## Usage

1. Open GitHub Copilot Chat in VS Code.
2. Use commands like `/fix` or `/copilot` to interact with the Playwright MCP tool.
3. Refer to the `playwright-mcp-config.json` file for configuration details.
