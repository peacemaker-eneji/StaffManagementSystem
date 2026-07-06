namespace StaffManagementSystem.Domain.Interfaces {
    /// <summary>
    /// Turns a user message (plus history) into a reply from the AI model.
    /// Implemented in Infrastructure using OllamaSharp / Microsoft.Extensions.AI.
    /// </summary>
    public interface IHolidayChatBot {
        Task<string> GetReplyAsync(
            string conversationId,
            string userMessage,
            IReadOnlyList<(string Role, string Content)> history,
            CancellationToken cancellationToken = default);
    }

}
