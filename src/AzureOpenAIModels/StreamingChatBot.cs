using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net;

namespace SK_DEV.AzureOpenAIModels
{
    internal static class StreamingChatBot
    {
        public static async Task InvokeChatBot(string modelId, string endpoint, string apiKey)
        {
            var azureKernelbuilder = Kernel.CreateBuilder();
            azureKernelbuilder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

            Kernel azureKernel = azureKernelbuilder.Build();

            var history = new ChatHistory();

            //Get reference to the chat completion service
            var chatCompletionService = azureKernel.GetRequiredService<IChatCompletionService>();

            OpenAIPromptExecutionSettings settings = new()
            {
                ChatSystemPrompt = "You are a friendly AI Assistant that answers in a friendly manner",
                Temperature = 0.9,
                MaxTokens = 1000,
            };

            //var reducer = new ChatHistoryTruncationReducer(targetCount: 2);
            var reducer = new ChatHistorySummarizationReducer(
                chatCompletionService,
                2,
                2
               );

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
                await foreach(StreamingChatMessageContent responseChunk in chatCompletionService.GetStreamingChatMessageContentsAsync(history, settings))
                {
                    Console.Write(responseChunk.Content);
                    fullMessage += responseChunk.Content;
                    usage = ((OpenAI.Chat.StreamingChatCompletionUpdate)responseChunk.InnerContent!).Usage;
                }

                //add response to chatHistory to maintain context
                history.AddAssistantMessage(fullMessage);
                Console.WriteLine($"\nToken Usage: InputTokens={usage?.InputTokenCount}, OutputTokens={usage?.OutputTokenCount}, Total={usage?.TotalTokenCount}");

                var reducedMessages = await reducer.ReduceAsync(history);

                Console.WriteLine($"\nChat history count . Message count: {history.Count}");

                if (reducedMessages != null)
                    history = [.. reducedMessages];
                    Console.WriteLine($"\nChat history was reduced. New message count: {history.Count}");
            }
        }
    }
}
