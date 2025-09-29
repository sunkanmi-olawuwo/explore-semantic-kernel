using Microsoft.Extensions.Configuration;
using SK_DEV.ModelProviders;

namespace SK_DEV
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

            var modelId = config["modelid"];
            var endpoint = config["endpoint"];
            var apiKey = config["apikey"];

            var openAIModelId = config["OpenAI:modelid"];
            var openAIEndpoint = config["OpenAI:apikey"];

            // Ensure required configuration values are not null or empty
            if (string.IsNullOrWhiteSpace(modelId))
                throw new InvalidOperationException("Missing required configuration value: modelid");
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException("Missing required configuration value: endpoint");
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Missing required configuration value: apikey");
            if (string.IsNullOrWhiteSpace(openAIModelId))
                throw new InvalidOperationException("Missing required configuration value: OpenAI:modelid");
            if (string.IsNullOrWhiteSpace(openAIEndpoint))
                throw new InvalidOperationException("Missing required configuration value: OpenAI:apikey");

            //await BasicChatBot.InvokeChatBot(modelId, endpoint, apiKey);
            //await StreamingChatBot.InvokeChatBot(modelId, endpoint, apiKey);
            await OpenAIChatBot.InvokeChatBot(openAIEndpoint, openAIModelId);
        }
    }
}
