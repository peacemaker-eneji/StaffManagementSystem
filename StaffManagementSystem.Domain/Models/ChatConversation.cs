using OllamaSharp.Models.Chat;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace StaffManagementSystem.Domain.Models {
    public class ChatConversation {
        private IChatClient? _chatClient;
        private List<ChatMessage> _history = new();
        private ChatOptions? _chatOptions;
        private readonly Microsoft.Extensions.AI.ChatRole _userChatRole = Microsoft.Extensions.AI.ChatRole.User;

        public string ConversationId { get; } = Guid.NewGuid().ToString();

        public IReadOnlyList<ChatMessage> History => _history;

        private ChatConversation() { } // must use create to instantiate

        public static ChatConversation Create(IChatClient chatClient, List<ChatMessage> history, List<AIFunction> tools) {
            ChatConversation chat = new ChatConversation();
            chat._chatClient = chatClient;
            chat._history = history;
            chat._chatOptions = new ChatOptions() {
                Tools = (IList<AITool>)tools,
                Temperature = 0.1f,
                ToolMode = ChatToolMode.Auto,
            };
            return chat;
        }

        public async Task<ChatResponse> SendAsync(string userMessage, CancellationToken ct = default) {
            _history.Add(new ChatMessage(_userChatRole, userMessage));
            ChatResponse response = await _chatClient.GetResponseAsync(_history, _chatOptions, ct);
            _history.AddRange(response.Messages);
            return response;
        }

        public async IAsyncEnumerable<ChatResponseUpdate> SendStreamingAsync(string userMessage, [EnumeratorCancellation] CancellationToken ct = default) {
            _history.Add(new ChatMessage(_userChatRole, userMessage));

            var updates = new List<ChatResponseUpdate>();
            await foreach (var update in _chatClient.GetStreamingResponseAsync(_history, _chatOptions, ct)) {
                updates.Add(update);
                yield return update;
            }

            // Reassemble the full response from the collected stream updates, and commit it to history.
            var finalResponse = updates.ToChatResponse();
            _history.AddRange(finalResponse.Messages);
        }
    }
}
