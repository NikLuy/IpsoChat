namespace IpsoChat.Web.Models
{
    public class ApiSettings
    {
        public ApiType Api { get; set; } = ApiType.AzureOpenAI;
        public string OpenAIModel { get; set; } = "gpt-4o";
        public string EmbeddingModel { get; set; } = "text-embedding-3-small";
    }

    public enum ApiType
    {
        OpenAI,
        AzureOpenAI
    }
}
