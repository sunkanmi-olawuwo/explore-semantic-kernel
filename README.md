# Explore Open AI Integration Using Semantic Kernel
A .NET 9 implementation of an intelligent conversational AI using Microsoft's Semantic Kernel framework. Features streaming responses from Azure OpenAI, advanced chat history management with summarization, and token usage tracking. A brief exploration into AI integration via the Semantic Kernel.

## Features

- 🚀 Streaming chat responses for real-time interaction
- 🧠 Built on Microsoft's Semantic Kernel framework
- 📊 Token usage tracking for monitoring consumption
- 📝 Intelligent chat history management with summarization
- 🔄 Context preservation using history reduction techniques

## Prerequisites

- .NET 9 SDK
- Azure OpenAI API access (endpoint and API key)
- A deployed model in Azure OpenAI service

## Getting Started

1. Clone the repository
2. Configure your Azure OpenAI credentials in the `appsettings.json`
3. Build and run the application


## How It Works

The implementation uses Semantic Kernel's chat completion capabilities with:

1. Streaming responses for real-time interaction
2. Chat history management to maintain context
3. Advanced history reduction techniques (summarization)
4. Token usage tracking for monitoring API consumption

## Dependencies

- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.OpenAI
