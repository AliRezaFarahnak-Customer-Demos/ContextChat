Demo of MCP STIDO and MCP SSE

dotnet user-secrets set "MODEL" "gpt-4o-mini"
dotnet user-secrets set "ENDPOINT" "https://aialfarahn5634093468.openai.azure.com"
dotnet user-secrets set "API_KEY" "qVIbymH3HWufDrIXL6tkf5p9d3ha4ej7kGDkigQsSnMnFkicgmcMJQQJ99BCACfhMk5XJ3w3AAAAACOGvnUO"
```



For GitHub Copilot within VS Code, you should instead set the key as the `x-functions-key` header in `mcp.json`, and you would just use `https://<funcappname>.azurewebsites.net/runtime/webhooks/mcp/sse` for the URL. The following example uses an input and will prompt you to provide the key when you start the server from VS Code:

```json
{
    "servers": {
        "my-mcp-server": {
            "type": "sse",
            "url": "<funcappname>.azurewebsites.net/runtime/webhooks/mcp/sse",
            "headers": {
                "x-functions-key": "${input:functions-mcp-extension-system-key}"
            }
        }
    }
}
