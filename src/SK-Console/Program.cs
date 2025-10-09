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
            var openAIApiKey = config["OpenAI:apikey"];

            var gitHubToken = config["GitHub:apikey"];
            var gitHubEndpoint = config["GitHub:endpoint"];
            var gitHubOpenAIModel = config["GitHub:modelid"];


            //await BasicChatBot.InvokeChatBot(config["modelid"], config["endpoint"], config["apikey"]);
            //await StreamingChatBot.InvokeChatBot(config["modelid"], config["endpoint"], config["apikey"]);
            //await OpenAIChatBot.InvokeChatBot(config["OpenAI:apikey"], config["OpenAI:modelid"]);
            //await GitHubOpenAIModels.InvokeChatBot(config["GitHub:apikey"],  config["GitHub:modelid"], config["GitHub:endpoint"]);
        }
    }
}
