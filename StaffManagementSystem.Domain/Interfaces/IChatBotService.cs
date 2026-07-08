using Microsoft.Extensions.AI;
using StaffManagementSystem.Domain.Models;
using System.Runtime.CompilerServices;

namespace StaffManagementSystem.Domain.Interfaces {
    public interface IChatBotService {
        // Starts a brand new conversation.
        ChatConversation StartConversation(params String[] toolCategories);

        // Resumes a conversation from previously persisted history
        ChatConversation ResumeConversation(List<ChatMessage> history, params String[] toolCategories);

        IAsyncEnumerable<ChatResponseUpdate> GetWelcomeNotifications(string? userName, CancellationToken ct);
    }

}
