using Backend.ChatServer;
using Backend.ChatServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ChatServer.ApiDtos;
using Shared.ChatServer.HubEvents;
using System.Net;

namespace Backend.Controllers
{
    [Route("api/chat/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomsController(
        ChatRoomManager chatRoomManager,
        ChatServerService chatServerService) : Controller
    {
        [HttpPost]
        public async Task<ActionResult<CreateRoomResponse>> CreateRoomAsync([FromBody] CreateRoomRequest request)
        {
            var newRoom = new ChatRoom
            {
                Id = chatRoomManager.GetNextChatRoomId(),
                Name = request.Name,
                Members = request.Members!
            };

            if (!chatRoomManager.CreateChatRoom(newRoom))
            {
                var errorResponse = new CreateRoomResponse
                {
                    Success = false,
                    ErrorMessage = "Cannot create chat room",
                    Id = null,
                    Name = newRoom.Name,
                    Members = newRoom.Members
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            await chatServerService.NotifyChatRoomCreated(new ChatRoomCreatedEvent
            {
                Id = newRoom.Id,
                Name = newRoom.Name!,
                Members = newRoom.Members
            });

            return Ok(new CreateRoomResponse
            {
                Success = true,
                Id = newRoom.Id,
                Name = newRoom.Name,
                Members = newRoom.Members
            });
        }

        [HttpPost("{roomId}/member")]
        public async Task<ActionResult<AddRoomMemberResponse>> AddMemberAsync(long roomId, [FromBody] AddRoomMemberRequest request)
        {
            var currentUserName = User.Identity?.Name;

            if (!chatRoomManager.IsMemberInChatRoom(roomId, currentUserName!))
            {
                var errorResponse = new AddRoomMemberResponse
                {
                    Success = false,
                    ErrorMessage = "You are not a member of the chat room",
                    RoomId = roomId,
                    UserId = request.UserId,
                };
                return StatusCode((int)HttpStatusCode.Forbidden, errorResponse);
            }

            var room = chatRoomManager.GetChatRoom(roomId);
            if (room is null)
            {
                var errorResponse = new AddRoomMemberResponse
                {
                    Success = false,
                    ErrorMessage = "Room not found",
                    RoomId = roomId,
                    UserId = request.UserId,
                };
                return StatusCode((int)HttpStatusCode.NotFound, errorResponse);
            }

            if (!chatRoomManager.AddMemberToChatRoom(roomId, request.UserId))
            {
                var errorResponse = new AddRoomMemberResponse
                {
                    Success = false,
                    ErrorMessage = "Cannot add member to room",
                    RoomId = roomId,
                    UserId = request.UserId,
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            await chatServerService.NotifyChatRoomMemberAdded(roomId, request.UserId);

            return Ok(new AddRoomMemberResponse
            {
                Success = true,
                RoomId = roomId,
                UserId = request.UserId,
            });
        }

        [HttpDelete("{roomId}/member")]
        public async Task<ActionResult<AddRoomMemberResponse>> RemoveMemberAsync(long roomId, [FromBody] AddRoomMemberRequest request)
        {
            var currentUserName = User.Identity?.Name;

            if (!chatRoomManager.IsMemberInChatRoom(roomId, currentUserName!))
            {
                var errorResponse = new AddRoomMemberResponse
                {
                    Success = false,
                    ErrorMessage = "You are not a member of the chat room",
                    RoomId = roomId,
                    UserId = request.UserId,
                };
                return StatusCode((int)HttpStatusCode.Forbidden, errorResponse);
            }

            var room = chatRoomManager.GetChatRoom(roomId);
            if (room is null)
            {
                var errorResponse = new AddRoomMemberResponse
                {
                    Success = false,
                    ErrorMessage = "Room not found",
                    RoomId = roomId,
                    UserId = request.UserId,
                };
                return StatusCode((int)HttpStatusCode.NotFound, errorResponse);
            }

            if (!chatRoomManager.RemoveMemberFromChatRoom(roomId, request.UserId))
            {
                var errorResponse = new AddRoomMemberResponse
                {
                    Success = false,
                    ErrorMessage = "Cannot remove member from room",
                    RoomId = roomId,
                    UserId = request.UserId,
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            await chatServerService.NotifyChatRoomMemberRemoved(roomId, request.UserId);

            return Ok(new AddRoomMemberResponse
            {
                Success = true,
                RoomId = roomId,
                UserId = request.UserId,
            });
        }

        [HttpDelete("{roomId}")]
        public async Task<ActionResult<DeleteRoomResponse>> DeleteRoomAsync(long roomId)
        {
            var currentUserName = User.Identity?.Name;

            if (!chatRoomManager.IsMemberInChatRoom(roomId, currentUserName!))
            {
                var errorResponse = new DeleteRoomResponse
                {
                    Success = false,
                    ErrorMessage = "You are not a member of the chat room",
                    Id = null,
                    Name = null,
                    Members = null
                };
                return StatusCode((int)HttpStatusCode.Forbidden, errorResponse);
            }

            var room = chatRoomManager.GetChatRoom(roomId);

            if (room is null)
            {
                var errorResponse = new DeleteRoomResponse
                {
                    Success = false,
                    Id = null,
                    Name = null,
                    Members = null
                };
                return StatusCode((int)HttpStatusCode.NotFound, errorResponse);
            }

            if (!chatRoomManager.RemoveChatRoom(roomId))
            {
                var errorResponse = new DeleteRoomResponse
                {
                    Success = false,
                    Id = room.Id,
                    Name = room.Name,
                    Members = room.Members
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, errorResponse);
            }

            await chatServerService.NotifyChatRoomDeleted(new ChatRoomDeletedEvent
            {
                Id = room.Id,
                Name = room.Name!,
                Members = room.Members
            });

            return Ok(new DeleteRoomResponse
            {
                Success = true,
                Id = room.Id,
                Name = room.Name,
                Members = room.Members
            });
        }

        [HttpGet]
        public ActionResult<GetRoomsResponse> GetMemberRooms()
        {
            var currentUserName = User.Identity?.Name;

            var rooms = chatRoomManager.GetMemberChatRooms(currentUserName!);

            var response = new GetRoomsResponse
            {
                Success = true,
                Rooms = rooms.Count > 0 ? [.. rooms.Select(room => new GetRoomsItem
                {
                    Id = room.Id,
                    Name = room.Name,
                    Members = room.Members
                })] : []
            };

            return Ok(response);
        }
    }
}
