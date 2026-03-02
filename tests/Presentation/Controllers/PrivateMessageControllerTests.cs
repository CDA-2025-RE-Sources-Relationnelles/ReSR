using System.Data.Common;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Presentation.Api.Users.Controllers;
using ReSR.Presentation.Api.Users.ValueObjects.Messages;

namespace ReSR.Presentation.Api.Tests.Controllers;

public class PrivateMessageControllerTests
{
    private readonly Mock<IPrivateMessageService> service = new();
    private readonly PrivateMessageController controller;

    public PrivateMessageControllerTests()
    {
        controller = new PrivateMessageController(service.Object);

        var userId = Id.New().ToString();
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId), new Claim(ClaimTypes.Role, "User")]));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext{ User = user}
        };
    }

    [Fact]
    public async Task SendMessageAsync_Should_ReturnCreatedMessage()
    {
        //Arrange
        var dto = new PrivateMessageController.SendMessageDto("Hello", Id.New());
        var message = PrivateMessage.TryCreate(
            new Mock<ReSR.Domain.Aggregates.Account.User>().Object,
            new Mock<ReSR.Domain.Aggregates.Accounts.User>().Object, "Hello").Unwrap();
        
        service.Setup(s => s.TrySendAsync(It.IsAny<Id>(), It.IsAny<Id>(), "Hello"))
            .returnsAsync(WebResponse.Success(message));

        //Act
        var result = await controller.SendMessageAsync(dto);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resource = Assert.IsType<PrivateMessageResource>(okResult.Value);
        Assert.Equal("Hello", resource.Content);
    }

    [Fact]
    public async Task GetMessagesAsync_Should_ReturnAllMessages()
    {
        //Arrange
        var userId = Id.Parse(controller.User.GetUserId()!.Value.ToString());
        var messages = new List<PrivateMessage>
        {
            PrivateMessage.TryCreate(
                new Mock<ReSR.Domain.Aggregates.Accounts.User>().Object,
                new Mock<ReSR.Domain.Aggregates.Accounts.User>().Object, "msg1").Unwrap()
        };

        service.Setup(s => s.TryGetAll(userId)).ReturnsAsync(Response.Success(messages));

        //Act
        var resuslt = await controller.GetMessagesAsync();

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resources = Assert.IsAssignableFrom<IEnumerable<PrivateMessageResource>>(okResult.Value);
        Assert.Single(resources);    
    
    }
}