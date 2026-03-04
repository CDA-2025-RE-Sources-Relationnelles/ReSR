using Moq;
using Xunit;
using ReSR.Application.Services.Users.Implementations;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;
using FluentResponse;
using FluentResponse.Interfaces;
using static FluentResponse.Extensions;

namespace ReSR.Tests.Application.Services
{
    public class PrivateMessageServiceTests
    {
        private readonly Mock<IPrivateMessageRepository> _messageRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly PrivateMessageService _service;

        public PrivateMessageServiceTests()
        {
            _messageRepoMock = new Mock<IPrivateMessageRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _service = new PrivateMessageService(_messageRepoMock.Object, _userRepoMock.Object);
        }

        [Fact]
        public async Task TrySendAsync_ShouldReturnSuccess_WhenUsersExist()
        {
            // ARRANGE
            uint senderId = 1;
            uint receiverId = 2;
            var sender = new User { Id = senderId };
            var receiver = new User { Id = receiverId };
            var content = "Hello";

            _userRepoMock.Setup(x => x.TryGetAsync(senderId))
                         .ReturnsAsync(Response.Success(sender));
            _userRepoMock.Setup(x => x.TryGetAsync(receiverId))
                         .ReturnsAsync(Response.Success(receiver));

            // ACT
            var result = await _service.TrySendAsync(senderId, receiverId, content);

            // ASSERT
            Assert.True(result.OnSuccess(u => true).Unwrap());
        }

        [Fact]
        public async Task TrySendAsync_ShouldFail_WhenSenderMissing()
        {
            // ARRANGE
            uint senderId = 1;
            uint receiverId = 2;

            _userRepoMock.Setup(x => x.TryGetAsync(senderId))
                        .ReturnsAsync(Response.Failure<User>("Sender not found"));

            // ACT
            var result = await _service.TrySendAsync(senderId, receiverId, "Hi");

            // ASSERT

            bool successCalled = false;
            string? errorMessage = null;

            result
                .OnSuccess(_ => successCalled = true)
                .OnFailure(err => errorMessage = err.Message);

            Assert.False(successCalled);
            Assert.Equal("Sender not found", errorMessage);
        }

        [Fact]
        public async Task TryGetMessagesBetweenAsync_ShouldReturnOrderedMessages()
        {
            // ARRANGE
            uint senderId = 1;
            uint receiverId = 2;

            var messages = new List<PrivateMessage>
            {
                new PrivateMessage { Content = "2", Id = 2 },
                new PrivateMessage { Content = "1", Id = 1 }
            };

            _messageRepoMock.Setup(x => x.GetMessagesBetweenUsersAsync(senderId, receiverId))
                            .ReturnsAsync(Response.Success(messages));

            // ACT
            var result = await _service.TryGetMessagesBetweenAsync(senderId, receiverId);

            // ASSERT
            var msgs = result.OnSuccess(m => m).Unwrap();
            Assert.Equal(2, msgs.Count());
            Assert.Equal("1", msgs.First().Content);
            Assert.Equal("2", msgs.Last().Content);
        }
    }
}