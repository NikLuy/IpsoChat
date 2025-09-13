using IpsoChat.Web.Components;
using IpsoChat.Web.Models;
using IpsoChat.Web.Services;
using IpsoChat.Web.Services.Ingestion;
using Microsoft.Extensions.AI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Load API settings from configuration with host override support
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();

// Configure AI client based on ApiSettings with backward compatible connection string names
if (apiSettings.Api == ApiType.OpenAI)
{
    var openaiClient = builder.AddOpenAIClient("openai-api");
    openaiClient.AddChatClient(apiSettings.OpenAIModel)
        .UseFunctionInvocation()
        .UseOpenTelemetry(configure: c =>
            c.EnableSensitiveData = builder.Environment.IsDevelopment());
    openaiClient.AddEmbeddingGenerator(apiSettings.EmbeddingModel);
}
else // AzureOpenAI
{
    var azureOpenaiClient = builder.AddAzureOpenAIClient("openai"); // Keep "openai" for backward compatibility
    azureOpenaiClient.AddChatClient(apiSettings.OpenAIModel)
        .UseFunctionInvocation()
        .UseOpenTelemetry(configure: c =>
            c.EnableSensitiveData = builder.Environment.IsDevelopment());
    azureOpenaiClient.AddEmbeddingGenerator(apiSettings.EmbeddingModel);
}

builder.AddQdrantClient("vectordb");
builder.Services.AddQdrantCollection<Guid, IngestedChunk>("data-defaultchat-chunks");
builder.Services.AddQdrantCollection<Guid, IngestedDocument>("data-defaultchat-documents");
builder.Services.AddScoped<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();

builder.Services.AddSingleton<IngestionProgressService>();
builder.Services.AddHostedService<BackgroundIngestionService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// By default, we ingest PDF files from the /wwwroot/Data directory. You can ingest from
// other sources by implementing IIngestionSource.
// Important: ensure that any content you ingest is trusted, as it may be reflected back
// to users or could be a source of prompt injection risk.


app.Run();
