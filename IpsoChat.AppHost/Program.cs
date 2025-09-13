var builder = DistributedApplication.CreateBuilder(args);

// You will need to set the connection string to your own value
// You can do this using Visual Studio's "Manage User Secrets" UI, or on the command line:
//   cd this-project-directory
//   dotnet user-secrets set ConnectionStrings:openai "Endpoint=https://models.inference.ai.azure.com;Key=YOUR-API-KEY"
//   dotnet user-secrets set ConnectionStrings:openai-api "Key=YOUR-OPENAI-API-KEY"
var openai = builder.AddConnectionString("openai");
var openaiApi = builder.AddConnectionString("openai-api");

var vectorDB = builder.AddQdrant("vectordb")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);


var webApp = builder.AddProject<Projects.IpsoChat_Web>("ipsochat-web");
webApp.WithReference(openai);
webApp.WithReference(openaiApi);
webApp
    .WithReference(vectorDB)
    .WaitFor(vectorDB);

// Pass ApiSettings configuration from AppHost to Web
var apiSettings = builder.Configuration.GetSection("ApiSettings");
foreach (var setting in apiSettings.GetChildren())
{
    webApp.WithEnvironment($"ApiSettings__{setting.Key}", setting.Value ?? "");
}

builder.Build().Run();
