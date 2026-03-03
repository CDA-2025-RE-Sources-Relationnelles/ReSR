using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Application.Dtos;
using ReSR.Presentation.Api.Users.Mappers;
using ReSR.Presentation.Api.Users.Dtos;
using ReSR.Presentation.Api.Users.Extensions;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Messages;

namespace ReSR.Presentation.Api.Users.Controllers;

[ApiController]
[Route(ROUTE)]
public class PrivateMessageController : ControllerBase
{
    public const string ROUTE = "/private-messages";
    private readonly IPrivateMessageService messageService;

    public PrivateMessageController(IPrivateMessageService messageService)
    {
        this.messageService = messageService;
    }

    [HttpPost]
    [Authorize(Roles = nameof(Domain.Aggregates.Accounts.User))]
    public async Task<IActionResult> SendMessageAsync(SendMessageDto dto)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var result = await messageService.TrySendAsync(userId.Value, dto.ReceiverId, dto.Content);

        if (result is IFailure failure)
            return BadRequest(failure.Exception);

        var message = (result as ISuccess<PrivateMessage>)!.Value;
        return Ok(message.ToDto());
    }

    [HttpGet("{friendId}")]
    [Authorize(Roles = nameof(Domain.Aggregates.Accounts.User))]
    public async Task<IActionResult> GetConversationAsync(uint friendId)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var result = await messageService.TryGetConversationMessages(userId.Value, friendId);

        if (result is IFailure failure)
            return BadRequest(failure.Exception);

        var messages = (result as ISuccess<IEnumerable<PrivateMessage>>)! .Value;
        return Ok(messages.ToConversationDto(userId.Value));
    }
}