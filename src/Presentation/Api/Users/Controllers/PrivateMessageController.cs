using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
public class PrivateMessageController(IPrivateMessageService messageService) : ControllerBase {

    public const string ROUTE = "/private-messages";
    
    public readonly record struct SendMessageDto(string Content, Id ReceiverId);

    [HttpPost]
    [Authorize(Roles = nameof(User))]
    public Task<IResult> SendMessageAsync(SendMessageDto dto) => 
        messageService
            .TrySendAsync(User.GetUserId()!.Value, dto.ReceiverId, dto.Content)
            .ToResourceAsync<PrivateMessage, PrivateMessageResource>(Results.Ok);

    [HttpGet]
    [Authorize(Roles = nameof(User))]
    public Task<IResult> GetMessagesAsync() =>
        messageService
            .TryGetAll(User.GetUserId()!.Value)
            .ToResourceAsync<PrivateMessage, PrivateMessageResource>(Results.Ok);
}
