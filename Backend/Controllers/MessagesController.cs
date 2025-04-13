using Backend.ChatServer;
using Backend.ChatServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ChatServer.ApiDtos;
using Shared.ChatServer.HubEvents;
using System.Net;

namespace Backend.Controllers
{
    [Route("api/Rooms/{roomId}/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class MessagesController(
        ILogger<MessagesController> logger,
        ChatRoomManager chatRoomManager,
        ChatMessageManager chatMessageManager,
        ChatMessageReceiptManager chatMessageReceiptManager,
        ChatServerService chatServerService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<SendMessageResponse>> SendMessageAsync(long roomId, [FromBody] SendMessageRequest request)
        {
            var currentUserName = User.Identity?.Name;

            if (!chatRoomManager.IsMemberInChatRoom(roomId, currentUserName!))
            {
                var errorResponse = new SendMessageResponse
                {
                    Success = false,
                    ErrorMessage = "You are not a member of the room",
                    MessageId = request.MessageId,
                    CreatedAt = request.CreatedAt,
                    SentAt = null,
                    RoomId = roomId,
                    Sender = currentUserName
                };
                return StatusCode((int)HttpStatusCode.Forbidden, errorResponse);
            }

            if (chatMessageManager.IsMessageExists(roomId, request.MessageId))
            {
                var errorResponse = new SendMessageResponse
                {
                    Success = false,
                    ErrorMessage = "The same message was already sent",
                    MessageId = request.MessageId,
                    CreatedAt = request.CreatedAt,
                    SentAt = null,
                    RoomId = roomId,
                    Sender = currentUserName
                };
                return StatusCode((int)HttpStatusCode.Conflict, errorResponse);
            }

            var newMessage = new ChatMessage
            {
                Id = request.MessageId,
                RoomId = roomId,
                Message = request.Message!.Trim(),
                CreatedAt = request.CreatedAt,
                SentAt = DateTime.UtcNow,
                Sender = currentUserName
            };

            if (!chatMessageManager.AddMessage(newMessage))
            {
                var errorResponse = new SendMessageResponse
                {
                    Success = false,
                    ErrorMessage = "Cannot send the message",
                    MessageId = request.MessageId,
                    CreatedAt = request.CreatedAt,
                    SentAt = null,
                    RoomId = roomId,
                    Sender = currentUserName
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            await chatServerService.NotifyChatMessageSent(new ChatMessageSentEvent
            {
                MessageId = newMessage.Id,
                RoomId = newMessage.RoomId,
                Message = newMessage.Message!,
                CreatedAt = newMessage.CreatedAt,
                SentAt = newMessage.SentAt,
                Sender = newMessage.Sender
            });

            return Ok(new SendMessageResponse
            {
                Success = true,
                MessageId = newMessage.Id,
                CreatedAt = newMessage.CreatedAt,
                SentAt = newMessage.SentAt,
                RoomId = newMessage.RoomId,
                Sender = newMessage.Sender
            });
        }

        [HttpPatch("{messageId}/delivered")]
        public async Task<ActionResult> SetDeliveryReceipt(long roomId, Guid messageId)
        {
            var currentUserName = User.Identity?.Name;

            if (!chatRoomManager.IsMemberInChatRoom(roomId, currentUserName!))
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            if (!chatMessageManager.IsMessageExists(roomId, messageId))
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            var deliveryReceipt = new ChatMessageDeliveryReceipt
            {
                RoomId = roomId,
                MessageId = messageId,
                Recipient = currentUserName!,
                DeliveredAt = DateTime.UtcNow
            };

            if (!chatMessageReceiptManager.AddDeliveryReceipt(deliveryReceipt))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            await chatServerService.NotifyChatMessageDelivered(new ChatMessageDeliveredEvent
            {
                RoomId = deliveryReceipt.RoomId,
                MessageId = deliveryReceipt.MessageId,
                Recipient = deliveryReceipt.Recipient,
                DeliveredAt = deliveryReceipt.DeliveredAt
            });

            return Ok();
        }

        [HttpPatch("read")]
        public async Task<ActionResult> SetReadReceipt(long roomId)
        {
            logger.LogInformation("SetReadReceipt: roomId={roomId}", roomId);

            var currentUserName = User.Identity?.Name;

            if (!chatRoomManager.IsMemberInChatRoom(roomId, currentUserName!))
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }

            var messageIds = chatMessageManager.GetMessageIdsByRoomId(roomId);

            var unreadMessageIds = messageIds.Where(mid => !chatMessageReceiptManager.GetReadReceipts(roomId).Any(r => r.MessageId == mid && r.Recipient == currentUserName!));

            ChatMessageReadReceipt[] newReadReceipts = [.. unreadMessageIds.Select(mid => new ChatMessageReadReceipt
            {
                RoomId = roomId,
                MessageId = mid,
                Recipient = currentUserName!,
                ReadAt = DateTime.UtcNow
            })];

            chatMessageReceiptManager.AddReadReceipts(newReadReceipts);

            await chatServerService.NotifyChatMessageRead(new ChatMessageReadEvent
            {
                RoomId = roomId,
                MessageIds = [.. newReadReceipts.Select(r => r.MessageId)],
                Recipient = currentUserName!,
                ReadAt = DateTime.UtcNow
            });

            return Ok();
        }

        [HttpGet]
        public ActionResult<GetMessagesResponse> GetMessages(long roomId)
        {
            var currentUserName = User.Identity?.Name;

            if (!chatRoomManager.IsMemberInChatRoom(roomId, currentUserName!))
            {
                var errorResponse = new GetMessagesResponse
                {
                    Success = false,
                    ErrorMessage = "You are not a member of the room",
                    RoomId = roomId,
                    Messages = []
                };
                return StatusCode((int)HttpStatusCode.Forbidden, errorResponse);
            }

            var messages = chatMessageManager.GetMessagesByRoomId(roomId);
            var deliveryReceipts = chatMessageReceiptManager.GetDeliveryReceipts(roomId);
            var readReceipts = chatMessageReceiptManager.GetReadReceipts(roomId);

            var response = new GetMessagesResponse
            {
                Success = true,
                RoomId = roomId,
                Messages = [.. messages.Select(m => new GetMessagesItem
                {
                    Id = m.Id,
                    Message = m.Message,
                    CreatedAt = m.CreatedAt,
                    SentAt = m.SentAt,
                    Sender = m.Sender,
                    DeliveryReceipts = [.. deliveryReceipts.Where(r => r.MessageId == m.Id).Select(r => new GetMessagesDeliveryReceiptItem
                    {
                        MessageId = r.MessageId,
                        Recipient = r.Recipient,
                        DeliveredAt = r.DeliveredAt
                    })],
                    ReadReceipts = [.. readReceipts.Where(r => r.MessageId == m.Id).Select(r => new GetMessagesReadReceiptItem
                    {
                        MessageId = r.MessageId,
                        Recipient = r.Recipient,
                        ReadAt = r.ReadAt
                    })]
                })]
            };

            return Ok(response);
        }
    }
}
