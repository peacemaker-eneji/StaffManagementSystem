using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Infrastructure.Persistence.Stores;
using StaffManagementSystem.Infrastructure.Services;

namespace StaffManagementSystem.Infrastructure.Extensions {
    public static class ChatbotExtension {
        public static IServiceCollection AddChatBotService(this IServiceCollection services) {
            var ollama = new OllamaApiClient("http://localhost:11434", "llama3.2:latest");
            services.AddMemoryCache();
            services.AddChatClient(ollama)
                .UseFunctionInvocation();
            services.AddSingleton<IChatConversationCacheStore, ChatConversationCacheStore>();
            services.AddSingleton<IAiToolsStore, AiToolsStore>();
            services.AddSingleton<IChatBotService, ChatBotService>();

            return services;
        }
    }
}
