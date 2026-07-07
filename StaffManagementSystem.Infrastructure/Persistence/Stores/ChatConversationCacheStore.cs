using Microsoft.Extensions.Caching.Memory;
using StaffManagementSystem.Domain.Models;
using StaffManagementSystem.Domain.Interfaces;

namespace StaffManagementSystem.Infrastructure.Persistence.Stores {

    // Keeps conversations alive in memory keyed by id, with a sliding idle timeout. 
    // Every TryGet/GetOrStart call resets the timer;
    // once nothing touches a conversation for _idleTimeout,
    // the cache evicts it.
    public class ChatConversationCacheStore : IChatConversationCacheStore {
        private readonly IChatBotService _chatBot;
        private readonly MemoryCache _cache;
        private static readonly TimeSpan _idleTimeout = TimeSpan.FromMinutes(20);
        private static readonly TimeSpan _expTimeout = TimeSpan.FromHours(4);

        public ChatConversationCacheStore(IChatBotService chatBot) {
            _chatBot = chatBot;
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public ChatConversation GetOrStart(Guid conversationId) {
            return _cache.GetOrCreate(conversationId, entry => {
                entry.SlidingExpiration = _idleTimeout;
                entry.AbsoluteExpirationRelativeToNow = _expTimeout;

                var conversation = _chatBot.StartConversation();
                return conversation;
            })!;
        }

        public ChatConversation? TryGet(Guid conversationId) {
            return _cache.TryGetValue(conversationId, out ChatConversation? conversation) ? conversation : null;
        }

        public async Task EndAsync(Guid conversationId) {
            if (_cache.TryGetValue(conversationId, out ChatConversation? conversation)) _cache.Remove(conversationId);
        }
    }

}
