using Microsoft.Extensions.AI;
using StaffManagementSystem.Domain.Models;
using StaffManagementSystem.Domain.Interfaces;

namespace StaffManagementSystem.Infrastructure.Services {

    // Long-lived chatbot service. Wraps an IChatClient (ideally one built with .UseFunctionInvocation() so tool-call loops are resolved.
    // also hands out short-lived ChatConversation instances.
    /// Register this as a singleton (or scoped, if your IChatClient is scoped).
    /// </summary>
    public sealed class ChatBotService : IChatBotService {
        private readonly IChatClient _chatClient;
        private readonly IAiToolsStore _aiToolsStore;
        private readonly static string _systemPrompt = """
            You are a helpful assistant that answers questions about holidays and calendar events.
            Always call the GetHolidays tool to look up real data instead of guessing dates from memory.
            Keep answers concise and mention specific dates when relevant.
            If the tool returns no results, say so plainly instead of making something up.
            """;

        public ChatBotService(IChatClient chatClient, IAiToolsStore aiToolsStore) {
            _chatClient = chatClient;
            _aiToolsStore = aiToolsStore;
        }

        public ChatConversation StartConversation(params String[] toolCategories) {
            var history = new List<ChatMessage>();
            history.Add(new ChatMessage(ChatRole.System, _systemPrompt));
            return ChatConversation.Create(_chatClient, history, _aiToolsStore.GetTools(toolCategories));
        }

        public ChatConversation ResumeConversation(List<ChatMessage> history, params string[] toolCategories) {
            return ChatConversation.Create(_chatClient, history, _aiToolsStore.GetTools(toolCategories));
        }
    }
}
