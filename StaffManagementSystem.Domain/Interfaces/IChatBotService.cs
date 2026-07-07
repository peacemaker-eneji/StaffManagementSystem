using Microsoft.Extensions.AI;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Domain.Interfaces {
    public interface IChatBotService {
        // Starts a brand new conversation.
        ChatConversation StartConversation(params String[] toolCategories);

        // Resumes a conversation from previously persisted history
        ChatConversation ResumeConversation(List<ChatMessage> history, params String[] toolCategories);
    }

}
