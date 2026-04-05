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
) : ControllerBase
{
    public const string ROUTE = "/private-messages";

    #region DTO
    #endregion

    #region ROUTES
    #endregion
}