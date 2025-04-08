# Context Chat - MCP Server Implementation

This project demonstrates how to implement a Message Channel Protocol (MCP) server using Azure Functions. MCP enables standardized communication between client applications and AI services, making it easier to build robust and scalable AI-powered applications.

## Architecture

### Before MCP Implementation

The diagram below shows a traditional architecture before implementing MCP, where clients communicate directly with backend services using various protocols:

### Traditional Architecture
```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#FFFFFF' }}}%%
graph LR
    subgraph  
        Client["Client Application"]
        
        BE["<span style='display:inline-block; margin-right:10px'><img src='FunctionApp.svg' width='30' height='30' /></span><span style='display:inline-block; margin-right:10px'><img src='APIManager.svg' width='30' height='30' /></span><span style='display:inline-block'><img src='VirtualMachine.svg' width='30' height='30' /></span><br>Backend + Orchestrator<br>Gen AI"]
        
        Client <-->|HTTP/REST/SOCKET| BE
    end

    %% Azure-themed styling
    style Client fill:#000000,stroke:#000000,color:white
    style BE fill:#000000,stroke:#000000,color:white
```

### Model Context Protocol
```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'background': '#FFFFFF' }}}%%
graph LR
    subgraph  
        Client["MCP Client + Orchestrator GenAI"]
        A["<img src='VirtualMachine.svg' width='36' height='36' /><br>Local Server MCP<br>Tools"]
        B["<span style='display:inline-block; margin-right:10px'><img src='FunctionApp.svg' width='30' height='30' /></span><span style='display:inline-block'><img src='APIManager.svg' width='30' height='30' /></span><br>Remote Server MCP<br>Tools"]
        C["<img src='VirtualMachine.svg' width='36' height='36' /><br>Local Server MCP<br>Tools"]
        
        Client <-->|MCP Protocol| A
        Client <-->|MCP Protocol| B
        Client <-->|MCP Protocol| C
        
        A <--> D[Files]
        B <--> E[Cloud Native Resources]
        C <-->|Web APIs| F[Rest API]
    end
    F -->|Internet| C

    %% Azure-themed styling
    style Client fill:#000000,stroke:#000000,color:white
    style A fill:#000000,stroke:#000000,color:white
    style B fill:#000000,stroke:#000000,color:white
    style C fill:#000000,stroke:#000000,color:white
    style D fill:#0078D4,stroke:#0078D4,color:white
    style E fill:#0078D4,stroke:#0078D4,color:white
    style F fill:#0078D4,stroke:#0078D4,color:white
```
## Prerequisites

- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) for deployment
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local#install-the-azure-functions-core-tools)
- Azure subscription

## Getting Started

### Local Development

1. Clone this repository
2. Configure user secrets for local development:

```bash
dotnet user-secrets set "MODEL" "gpt-4o-mini"
dotnet user-secrets set "ENDPOINT" "<your-openai-endpoint>"
dotnet user-secrets set "API_KEY" "<your-api-key>"

{
    "servers": {
        "my-mcp-server": {
            "type": "sse",
            "url": "<function-app-name>.azurewebsites.net/runtime/webhooks/mcp/sse",
            "headers": {
                "x-functions-key": "${input:functions-mcp-extension-system-key}"
            }
        }
    }
}
```