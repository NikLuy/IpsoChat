# IpsoChat - AI-Powered Knowledge Base Chat

IpsoChat is an intelligent chat application built with Blazor Server and .NET 9 that allows users to ask questions about documents and receive AI-powered responses with citations. The application uses semantic search to find relevant information from ingested documents and provides contextual answers.

This project demonstrates how to chat with custom data using an AI language model. Please note that this template is currently in an early preview stage. If you have feedback, please take a [brief survey](https://aka.ms/dotnet-chat-templatePreview2-survey).

## Features

- 🤖 AI-powered chat with document-based responses
- 📄 PDF document ingestion and processing
- 🔍 Semantic search with vector embeddings
- 📚 Citation tracking and source references
- 🎨 Modern Blazor UI with Bootstrap styling
- 🔄 Real-time chat interface with auto-scroll
- 📊 Background document processing
- 🌐 Built with .NET Aspire for cloud-ready deployment
- ⚙️ **Flexible AI Provider Support** - Switch between OpenAI and Azure OpenAI/GitHub Models

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Aspire orchestration and Qdrant)

>[!NOTE]
> Before running this project you need to configure the API keys or endpoints for the providers you have chosen. See below for details specific to your choices.

## Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/NikLuy/IpsoChat.git
cd IpsoChat
```

### 2. Choose Your AI Provider

IpsoChat supports two AI providers:
- **Azure OpenAI / GitHub Models** (recommended for development)
- **OpenAI API** (requires paid OpenAI account)

#### Option A: GitHub Models (Free Tier)

To use GitHub Models for AI inference, you need to create a Personal Access Token:

**Step 1: Navigate to GitHub Token Settings**
1. Go to [GitHub](https://github.com) and sign in to your account
2. Click on your profile picture in the top right corner
3. Select **Settings** from the dropdown menu
4. In the left sidebar, scroll down and click on **Developer settings**
5. Click on **Personal access tokens**
6. Select **Tokens (classic)**

![Personal Access Tokens Navigation](Doku/images/personal-access-tokens.png)

**Step 2: Generate New Token**
1. Click the **Generate new token** dropdown button
2. Choose **Generate new token (classic)** for general use

![Generate New Token Options](Doku/images/generate-new-token.png)

3. Fill out the token details:
   - **Note**: Give your token a descriptive name (e.g., "IpsoChat AI Models")
   - **Expiration**: Set as needed (recommended: 90 days or custom)
   - **Scopes**: For GitHub Models API access, the token should not have any scopes or permissions (leave all checkboxes unchecked)

4. Click **Generate token**

**Step 3: Copy and Store Your Token**
⚠️ **Important**: Copy your token immediately! You won't be able to see it again.

The token will look something like: `ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`

Learn more about [prototyping with AI models using GitHub Models](https://docs.github.com/github-models/prototyping-with-ai-models).

#### Option B: OpenAI API

If you prefer to use OpenAI directly:
1. Go to [OpenAI API](https://platform.openai.com/api-keys)
2. Create a new API key
3. Copy the API key (starts with `sk-`)

### 3. Configure User Secrets

The application uses .NET User Secrets to store sensitive configuration. Set up your secrets using one of these methods:

#### Option A: Using Visual Studio
1. Right-click on the `IpsoChat.AppHost` project in Solution Explorer
2. Select **Manage User Secrets**
3. This opens a `secrets.json` file where you can store your API keys without them being tracked in source control

#### Option B: Using .NET CLI
Navigate to the `IpsoChat.AppHost` directory and run the appropriate commands:

**For GitHub Models / Azure OpenAI:**
```bash
dotnet user-secrets set "ConnectionStrings:openai" "Endpoint=https://models.inference.ai.azure.com;Key=YOUR_GITHUB_TOKEN_HERE"
dotnet user-secrets set "Parameters:vectordb-Key" "YOUR_VECTOR_DB_KEY_HERE"
```

**For OpenAI API:**
```bash
dotnet user-secrets set "ConnectionStrings:openai-api" "Key=YOUR_OPENAI_API_KEY_HERE"
dotnet user-secrets set "Parameters:vectordb-Key" "YOUR_VECTOR_DB_KEY_HERE"
```

**For Both (to easily switch):**
```bash
dotnet user-secrets set "ConnectionStrings:openai" "Endpoint=https://models.inference.ai.azure.com;Key=YOUR_GITHUB_TOKEN_HERE"
dotnet user-secrets set "ConnectionStrings:openai-api" "Key=YOUR_OPENAI_API_KEY_HERE"
dotnet user-secrets set "Parameters:vectordb-Key" "YOUR_VECTOR_DB_KEY_HERE"
```

### 4. Configure AI Provider

You can configure which AI provider to use and which models to use through the `ApiSettings` configuration:

#### In appsettings.json (Web project defaults):
```json
{
  "ApiSettings": {
    "Api": "AzureOpenAI",
    "OpenAIModel": "gpt-4o-mini",
    "EmbeddingModel": "text-embedding-3-small"
  }
}
```

#### In AppHost appsettings.json (Host overrides):
```json
{
  "ApiSettings": {
    "Api": "OpenAI",
    "OpenAIModel": "gpt-4o",
    "EmbeddingModel": "text-embedding-3-small"
  }
}
```

#### Available Options:

| Setting | Options | Description |
|---------|---------|-------------|
| `Api` | `"AzureOpenAI"` or `"OpenAI"` | Which AI provider to use |
| `OpenAIModel` | `"gpt-4o"`, `"gpt-4o-mini"`, `"gpt-3.5-turbo"`, etc. | Chat model to use |
| `EmbeddingModel` | `"text-embedding-3-small"`, `"text-embedding-3-large"`, etc. | Embedding model for document processing |

#### Environment Variable Override:
You can also override settings using environment variables:
```bash
# To use OpenAI
ApiSettings__Api=OpenAI

# To use Azure OpenAI / GitHub Models
ApiSettings__Api=AzureOpenAI

# To change models
ApiSettings__OpenAIModel=gpt-4o
ApiSettings__EmbeddingModel=text-embedding-3-large
```

### 5. Setting up a local environment for Qdrant

This project is configured to run Qdrant in a Docker container. Docker Desktop must be installed and running for the project to run successfully. A Qdrant container will automatically start when running the application.

Download, install, and run Docker Desktop from the [official website](https://www.docker.com/). Follow the installation instructions specific to your operating system.

Note: Qdrant and Docker are excellent open source products, but are not maintained by Microsoft.

### 6. Add Your Documents

1. Create a directory structure for your documents (if not already present):
   ```
   IpsoChat.Web/wwwroot/Data/SYEN/
   ```
2. Place your PDF documents in this directory
3. The application will automatically process these documents on startup through the background ingestion service

### 7. Run the Application

#### Using Visual Studio
1. Open the `.sln` file in Visual Studio
2. Set `IpsoChat.AppHost` as the startup project
3. Press `Ctrl+F5` or click the "Start" button in the toolbar to run the project

#### Using Visual Studio Code
1. Open the project folder in Visual Studio Code
2. Install the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) for Visual Studio Code
3. Once installed, open the `Program.cs` file in the IpsoChat.AppHost project
4. Run the project by clicking the "Run" button in the Debug view

#### Using .NET CLI
```bash
dotnet run --project IpsoChat.AppHost
```

The application will start and open in your default browser. You can access:
- **Chat Interface**: Main application for asking questions about your documents
- **Aspire Dashboard**: Monitoring and diagnostics for all services

## Trust the localhost certificate

Several .NET Aspire templates include ASP.NET Core projects that are configured to use HTTPS by default. If this is the first time you're running the project, an exception might occur when loading the Aspire dashboard. This error can be resolved by trusting the self-signed development certificate with the .NET CLI.

See [Troubleshoot untrusted localhost certificate in .NET Aspire](https://learn.microsoft.com/dotnet/aspire/troubleshooting/untrusted-localhost-certificate) for more information.

## Configuration

### Environment Variables

The application supports the following configuration options:

| Setting | Description | Default |
|---------|-------------|---------|
| `ConnectionStrings:openai` | Azure OpenAI/GitHub Models endpoint and key | Required for Azure OpenAI |
| `ConnectionStrings:openai-api` | OpenAI API key | Required for OpenAI |
| `Parameters:vectordb-Key` | Vector database authentication key | Required |
| `ApiSettings:Api` | AI provider (`OpenAI` or `AzureOpenAI`) | `AzureOpenAI` |
| `ApiSettings:OpenAIModel` | Chat model name | `gpt-4o-mini` |
| `ApiSettings:EmbeddingModel` | Embedding model name | `text-embedding-3-small` |

### Document Processing

- **Supported Formats**: PDF files
- **Processing**: Automatic background ingestion with progress tracking
- **Storage**: Documents are chunked and vectorized for semantic search
- **Location**: Place documents in `wwwroot/Data/SYEN/`

### Rate Limits

**GitHub Models (Free Tier):**
- 150 embedding requests per day
- For production use, consider upgrading to [Copilot Pro](https://github.com/pricing) (300 requests/day) or [paid usage](https://docs.github.com/en/billing/managing-billing-for-your-products/about-billing-for-github-models)

**OpenAI API:**
- Rate limits depend on your OpenAI plan
- Pay-per-use pricing

## Project Structure

```
IpsoChat/
├── IpsoChat.AppHost/          # Aspire orchestration project
├── IpsoChat.Web/              # Main Blazor application
│   ├── Components/            # Blazor components
│   │   ├── Pages/            # Page components (Chat, etc.)
│   │   └── Layout/           # Layout components
│   ├── Services/             # Application services
│   │   ├── Ingestion/        # Document processing services
│   │   └── AI integration    # SemanticSearch, etc.
│   ├── Models/               # Configuration models (ApiSettings)
│   └── wwwroot/              # Static files and documents
├── IpsoChat.ServiceDefaults/  # Shared service configuration
└── Doku/                     # Documentation and images
    └── images/               # Setup instruction screenshots
```

## Usage

1. **Start a Conversation**: Type your question in the chat input field
2. **Get AI Responses**: The system will search through your ingested documents and provide contextual answers
3. **View Citations**: Responses include citations showing which documents and pages were used as sources
4. **New Conversation**: Click "Neue Anfrage" (New Request) to start a fresh conversation

## Known Issues

### Errors running Ollama or Docker

A recent incompatibility was found between Ollama and Docker Desktop. This issue results in runtime errors when connecting to Ollama, and the workaround for that can lead to Docker not working for Aspire projects.

This incompatibility can be addressed by upgrading to Docker Desktop 4.41.1. See [ollama/ollama#9509](https://github.com/ollama/ollama/issues/9509#issuecomment-2842461831) for more information and a link to install the version of Docker Desktop with the fix.

## Troubleshooting

### Common Issues

**Issue**: Styles or JavaScript not loading after project rename
**Solution**: Clean and rebuild the solution:
```bash
dotnet clean
dotnet build
```

**Issue**: GitHub Models API authentication fails
**Solution**: Verify your GitHub token is correct and has the required permissions (should have no scopes selected)

**Issue**: Rate limiting with GitHub Models
**Solution**: You've hit the 150 requests/day limit. Either wait 24 hours, upgrade to Copilot Pro, or switch to OpenAI API

**Issue**: Documents not being processed
**Solution**: Check that PDF files are placed in the correct directory (`wwwroot/Data/SYEN/`) and the background service is running

**Issue**: Qdrant connection errors
**Solution**: Ensure Docker Desktop is running and the Qdrant container has started successfully

**Issue**: Wrong AI provider being used
**Solution**: Check your `ApiSettings` configuration and ensure the correct connection string is set in user secrets

### Logs and Diagnostics

- Use the Aspire Dashboard to monitor application health and service status
- Check the application logs for detailed error information
- Verify document ingestion progress through the application interface
- Monitor the background ingestion service for document processing status

### Configuration Examples

**Example 1: Development with GitHub Models**
```json
{
  "ApiSettings": {
    "Api": "AzureOpenAI",
    "OpenAIModel": "gpt-4o-mini",
    "EmbeddingModel": "text-embedding-3-small"
  }
}
```

**Example 2: Production with OpenAI API**
```json
{
  "ApiSettings": {
    "Api": "OpenAI",
    "OpenAIModel": "gpt-4o",
    "EmbeddingModel": "text-embedding-3-small"
  }
}
```

## Updating JavaScript dependencies

This template leverages JavaScript libraries to provide essential functionality. These libraries are located in the wwwroot/lib folder of the IpsoChat.Web project. For instructions on updating each dependency, please refer to the README.md file in each respective folder.

## Technologies Used

- **Frontend**: Blazor Server (.NET 9)
- **AI/ML**: Microsoft.Extensions.AI with configurable providers:
  - GitHub Models (Azure OpenAI endpoint)
  - OpenAI API
- **Vector Search**: Qdrant vector database
- **Document Processing**: PdfPig for PDF parsing
- **Orchestration**: .NET Aspire
- **UI Framework**: Bootstrap 5 with custom styling
- **Real-time Features**: SignalR for live chat updates

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## Learn More

To learn more about development with .NET and AI, check out the following links:

* [AI for .NET Developers](https://learn.microsoft.com/dotnet/ai/)
* [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
* [Blazor Documentation](https://learn.microsoft.com/aspnet/core/blazor/)
* [GitHub Models Documentation](https://docs.github.com/github-models/)
* [OpenAI API Documentation](https://platform.openai.com/docs/)

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
- Create an issue on [GitHub](https://github.com/NikLuy/IpsoChat/issues)
- Check the troubleshooting section above
- Review the Aspire Dashboard for diagnostic information
