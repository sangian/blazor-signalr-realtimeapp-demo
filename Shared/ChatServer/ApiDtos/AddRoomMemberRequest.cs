﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.ChatServer.ApiDtos;
public record AddRoomMemberRequest
{
    [JsonPropertyName("userId")]
    [Required(ErrorMessage = "UserId is required")]
    public required string UserId { get; init; }
}
