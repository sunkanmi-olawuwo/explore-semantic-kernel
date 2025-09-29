using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureAIInference;

namespace SK_DEV.ModelProviders
{
    internal class GitHubOpenAIModels
    {
        public static async Task InvokeChatBot(string apiKey, string modelId, string endpoint)
        {
            var Kernelbuilder = Kernel.CreateBuilder();
            Kernelbuilder.AddAzureAIInferenceChatCompletion(modelId, apiKey, new Uri(endpoint));

            Kernel kernel = Kernelbuilder.Build();

            var history = new ChatHistory(systemMessage: "You are a friendly AI Assistant that answers in a friendly manner");

            //Get reference to the chat completion service
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            AzureAIInferencePromptExecutionSettings settings = new()

            {
                Temperature = 0.9f,
                MaxTokens = 1500,
            };

            var reducer = new ChatHistoryTruncationReducer(targetCount: 2);
            /*var reducer = new ChatHistorySummarizationReducer(
                chatCompletionService,
                2,
                2
               );*/

            foreach (var attr in chatCompletionService.Attributes)
            {
                Console.WriteLine($"{attr.Key}: {attr.Value}");
            }

            while (true)
            {
                //Get input from user
                Console.Write("\nEnter your prompt: ");
                var prompt = Console.ReadLine();

                //Exit if prompt is empty or null
                if (string.IsNullOrEmpty(prompt)) break;

                string fullMessage = "";
                OpenAI.Chat.ChatTokenUsage? usage = null;

                //add user prompt to chat history
                history.AddUserMessage(prompt);
                await foreach (StreamingChatMessageContent responseChunk in chatCompletionService.GetStreamingChatMessageContentsAsync(history, settings))
                {
                    // Print response to console
                    Console.Write(responseChunk.Content);
                    fullMessage += responseChunk.Content;
                }

                // Add response to chat history
                history.AddAssistantMessage(fullMessage);

                // Reduce chat history if necessary
                var reduceMessages = await reducer.ReduceAsync(history);
                if (reduceMessages is not null)
                    history = new(reduceMessages);
            }
        }
    }
}
