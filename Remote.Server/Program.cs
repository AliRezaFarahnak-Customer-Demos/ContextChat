using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
    builder.ConfigureFunctionsWebApplication();
    builder.EnableMcpToolMetadata();
    builder.Build().Run();
