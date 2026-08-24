using MediatR;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;
using System.Runtime.CompilerServices;

namespace StaffManagementSystem.Api.Controllers {
    public record ReplyConversationDto(string ConversationId, string UserMessage);

    [ApiController]
    [Route("chatbot")]
    public class ChatBotController : ControllerBase {
        private readonly ILogger<ChatBotController> _logger;
        private readonly IChatBotService _chatBotService;
        private readonly IChatConversationCacheStore _conversationCacheStore;
        private readonly IMediator _mediator;

        public ChatBotController(IMediator mediator, 
            IChatBotService chatBotService, 
            IChatConversationCacheStore conversationCacheStore,
            ILogger<ChatBotController> logger) {
            _mediator = mediator;
            _logger = logger;
            _chatBotService = chatBotService;
            _conversationCacheStore = conversationCacheStore;
        }

        [HttpGet("notification")]
        public async Task<ActionResult> GenerateNotification([EnumeratorCancellation] CancellationToken ct = default) {
            var userName = User?.Identity?.Name;
            Response.ContentType = "text/event-stream";
            await foreach (var chunk in _chatBotService.GetWelcomeNotifications(userName, ct)) {
                await Response.WriteAsync(chunk.Text);
                await Response.Body.FlushAsync();
            }
            return new EmptyResult();
        }

        [HttpPost("start-conversation")]
        public async Task<ActionResult<ApiResponse<string>>> StartConversation() {
            var conversation = _chatBotService.StartConversation();
            var conversationId = Guid.NewGuid().ToString();
            _conversationCacheStore.Add(conversationId, conversation);
            var response = new ApiResponse<string> {
                Status = StatusCodes.Status201Created,
                Message = "Started Conversation Succesfully",
                Data = conversationId
            };
            return StatusCode(response.Status, response);
        }


        /// <summary>
        /// Uses a SSE response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("reply-conversation")]
        public async Task<ActionResult> ReplyConversation(ReplyConversationDto request, [EnumeratorCancellation] CancellationToken ct = default) {
            var conversation = _conversationCacheStore.Get(request.ConversationId);
            if (conversation is null) {
                var response = new ApiResponse {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Conversation not found",
                    Success = false
                };
                return StatusCode(response.Status, response);
            }

            Response.ContentType = "text/event-stream";

            await foreach (var chunk in conversation.SendStreamingAsync(request.UserMessage, ct)) {
                await Response.WriteAsync(chunk.Text);
                await Response.Body.FlushAsync();
            }

            return new EmptyResult();
        }

        [HttpPost("stop-conversation")]
        public async Task<ActionResult<ApiResponse>> StopConversation(string conversationId) {
            _conversationCacheStore.Remove(conversationId);
            var response = new ApiResponse {
                Status = StatusCodes.Status200OK,
                Message = "Conversation Stopped successfully"
            };
            return StatusCode(response.Status, response);
        }
    }
}
