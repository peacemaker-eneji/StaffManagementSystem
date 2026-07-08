using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Domain.Interfaces {

    // Tracks in-flight conversations by id and auto-expires them after a periodof inactivity
    public interface IChatConversationCacheStore {
        // Gets the existing conversation for this id, or starts a new one if none exists / it expired.
        void Add(string conversationId, ChatConversation conversation);

        // Gets the conversation if it's still alive, or null if it doesn't exist / already expired.
        ChatConversation? Get(string conversationId);

        // Ends a conversation early (before its idle timeout), disposing it immediately.
        void Remove(string conversationId);
    }
}
