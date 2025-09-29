using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SK_DEV.SemanticKernelFoundation
{
    internal static class BasicChatBot
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

                //add user prompt to chat history
                history.AddUserMessage(prompt);
                var response = await chatCompletionService.GetChatMessageContentAsync(history, settings);

                //add response to chatHistory to maintain context
                history.Add(response);

                OpenAI.Chat.ChatTokenUsage usage = ((OpenAI.Chat.ChatCompletion)response.InnerContent!).Usage;

                Console.WriteLine($"\nResponse: {response.Content}");
                Console.WriteLine($"\nToken Usage: InputTokens={usage.InputTokenCount}, OutputTokens={usage.OutputTokenCount}, Total={usage.TotalTokenCount}");


                var reducedMessages = await reducer.ReduceAsync(history);
                if (reducedMessages != null)
                    history = new(reducedMessages);
                Console.WriteLine($"\nChat history was reduced. New message count: {history.Count}");
            }
        }
    }
}
