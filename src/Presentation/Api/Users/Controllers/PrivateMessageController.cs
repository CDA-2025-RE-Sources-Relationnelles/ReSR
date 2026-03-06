using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Presentation.Api.Users.Extensions;
using ReSR.Presentation.Api.Users.ValueObjects.Messages;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Domain.Aggregates.Resources;

namespace ReSR.Presentation.Api.Users.Controllers;

[ApiController]
[Route(ROUTE)]
public class PrivateMessageController(
    IPrivateMessageService messageService
) : ControllerBase
{
    public const string ROUTE = "/private-messages";

    #region DTO

    public readonly record struct SendPrivateMessageDto(
        Id ReceiverId,
        string Content,
        Id? ResourceId
    );

    #endregion

    #region ROUTES

    [HttpPost]
    [Authorize(Roles = nameof(User))]
    [EndpointSummary("Send a private message to a user.")]
    public Task<IResult> SendAsync(SendPrivateMessageDto dto) =>
        messageService
            .TrySendAsync(
                User.GetUserId()!.Value,
                dto.ReceiverId,
                dto.Content,
                dto.ResourceId
            )
            .ToResourceAsync<PrivateMessage, PrivateMessageResource>(Results.Ok);

    [HttpGet("{friendId}")]
    [Authorize(Roles = nameof(User))]
    [EndpointSummary("Get all private messages between the authenticated user and a friend.")]
    public Task<IResult> GetMessagesAsync(Id friendId) =>
        messageService
            .TryGetMessagesBetweenAsync(
                User.GetUserId()!.Value,
                friendId
            )
            .ToResourceAsync<PrivateMessage, PrivateMessageResource>(Results.Ok);

    #endregion
}