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