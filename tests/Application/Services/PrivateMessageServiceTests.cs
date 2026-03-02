using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using FluentResponse;
using FluentResponse.Interfaces;
using Moq;
using ReSR.Application.Services.Users.Implementations;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;

namespace ReSR.Application.Tests.Services;

public class PrivateMessageServiceTests
{
    private readonly Mock<IRepository<PrivateMessag>> messageRepository = new();

    private readonly Mock<IReppository<User>> userRepository = new();

    private readonly PrivateMessageService service;

    public PrivateMessageServiceTests()
    {
        service = new PrivateMessageService(messageReopsitory.Object, userRepository.Object);
    }

    [Fact]
    public async Task TrySendAsync_Should_CreateMessage_When_ValidUsers()
    {
        //Arrange
        var sender = new Mock<User>();
        sender.SetupGet(x => x.Id).Returns(Id.New());

        var receiver = new Mock<User>();
        receiver.SetupGet(x => x.Id).Returns(Id.New());

        userRepository.Setup(r => r.TryGetAsync(sender.Object.Id)).ReturnsAsync(sender.Object);
        userRepository.Setup(r => r.TryGetAsync(receiver.Object.Id)).ReturnsAsync(receiver.Object);

        PrivateMessage? savedMessage = null;
        messageRepository.Setup(r => r.TryAddAsync(It.IsAny<PrivateMessage>()))
            .Returns<PrivateMessage>(m => { savedMessage = m; return Task.FromResult(Response.Success(m));
            });

        //Act
        var response = await service.TrySendAsync(sender.Object.Id, receiver.Object.Id, "Hello !");

        //Assert
        Assert.True(response.IsSuccess);
        Assert.NotNull(savedMessage);
        Assert.Equal(sender.Object, savedMessage.SentBy);
        Assert.Equal(receiver.Object,savedMessage.SentTo);
        Assert.Equal("Hello !", savedMessage.Content);
    }

    [Fact]
    public async Task TryGetAll_Should_ReturnMessagesForUser()
    {
        //Arrange
        var user = new Mock<User>();
        user.SetupGet(x => x.Id).Returns(Id.New());

        var msg1 = PrivateMessage.TryCreate(user.Object, new Mock<User>().Object, "msg1").Unwrap();
        var msg2 = PrivateMessage.TryCreate(new Mock<User>().Object, user.Object, "msg2").Unwrap();
        var allMessages = new List<PrivateMessage> {msg1, msg2};

        messageRepository.Setup(r.GetAllAsync(It.IsAny<Func<PrivateMessageServiceTests, bool>>()))
                        .ReturnsAsync((Func<PrivateMessageServiceTests, bool> predicate) => allMessages.Where(predicate));

        //Act
        var response = await service.TryGetAll(user.Object.Id);

        //Assert
        Assert.True(response.IsSuccess);
        Assert.Equal(2, response.Value.Count());
    }
}