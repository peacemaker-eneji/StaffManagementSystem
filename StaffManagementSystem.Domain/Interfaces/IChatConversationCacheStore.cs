using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Domain.Interfaces {

    // Tracks in-flight conversations by id and auto-expires them after a periodof inactivity
    public interface IChatConversationCacheStore {
        // Gets the existing conversation for this id, or starts a new one if none exists / it expired.
        ChatConversation GetOrStart(Guid conversationId);

        // Gets the conversation if it's still alive, or null if it doesn't exist / already expired.
        ChatConversation? TryGet(Guid conversationId);

        // Ends a conversation early (before its idle timeout), disposing it immediately.
        Task EndAsync(Guid conversationId);
    }
}
