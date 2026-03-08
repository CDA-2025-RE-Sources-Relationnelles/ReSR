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
using ReSR.Domain.Aggregates.Resources;

namespace ReSR.Tests.Application.Services
{
    public class PrivateMessageServiceTests
    {
        private readonly Mock<IPrivateMessageRepository> _messageRepoMock;
        private readonly Mock<IRepository<User>> _userRepoMock;

        private readonly Mock<IRepository<Resource>> _resourceRepoMock;
        private readonly PrivateMessageService _service;

        public PrivateMessageServiceTests()
        {
            _messageRepoMock = new Mock<IPrivateMessageRepository>();
            _userRepoMock = new Mock<IRepository<User>>();
            _resourceRepoMock = new Mock<IRepository<Resource>>();
            _service = new PrivateMessageService(_messageRepoMock.Object, _userRepoMock.Object, _resourceRepoMock.Object);
        }

        [Fact]
        public async Task TrySendAsync_ShouldReturnSuccess_WhenUsersAreFriends()
        {
            // ARRANGE
            uint senderId = 1;
            uint receiverId = 2;
            var sender = new User { Id = senderId };
            var receiver = new User { Id = receiverId };

            sender.LikedUsers.Add(receiver);
            sender.LikedBy.Add(receiver);
            receiver.LikedBy.Add(sender);
            receiver.LikedUsers.Add(sender);

            var content = "Hello";

            _userRepoMock.Setup(x => x.TryGetAsync(senderId))
                        .ReturnsAsync(Response.Success(sender));
            _userRepoMock.Setup(x => x.TryGetAsync(receiverId))
                        .ReturnsAsync(Response.Success(receiver));

            _messageRepoMock.Setup(x => x.TryAddAsync(It.IsAny<PrivateMessage>()))
                            .ReturnsAsync((PrivateMessage m) => Response.Success(m));

            // ACT
            var result = await _service.TrySendAsync(senderId, receiverId, content);

            // ASSERT
            bool successCalled = false;
            string? errorMessage = null;

            result.OnSuccess(_ => successCalled = true)
                .OnFailure(err => errorMessage = err.Message);

            Assert.True(successCalled);
            Assert.Null(errorMessage);
        }

        [Fact]
        public async Task TrySendAsync_ShouldFail_WhenUsersAreNotFriends()
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
            bool successCalled = false;
            string? errorMessage = null;

            result.OnSuccess(_ => successCalled = true)
                .OnFailure(err => errorMessage = err.Message);

            Assert.False(successCalled);
            Assert.Equal("Vous devez être amis pour vous envoyer des messages !", errorMessage);
        }

        [Fact]
        public async Task TrySendAsync_ShouldReturnSuccess_WithQuotedResource()
        {
            // ARRANGE
            uint senderId = 1;
            uint receiverId = 2;
            uint resourceId = 10;

            var sender = new User { Id = senderId };
            var receiver = new User { Id = receiverId };

            sender.LikedUsers.Add(receiver);
            sender.LikedBy.Add(receiver);
            receiver.LikedUsers.Add(sender);
            receiver.LikedBy.Add(sender);

            var resource = new TextResource { Id = resourceId, Title = "Test Resource" };

            var content = "Regarde cette ressource !";

            _userRepoMock.Setup(x => x.TryGetAsync(senderId))
                .ReturnsAsync(Response.Success(sender));

            _userRepoMock.Setup(x => x.TryGetAsync(receiverId))
                .ReturnsAsync(Response.Success(receiver));

            _resourceRepoMock.Setup(x => x.TryGetAsync(resourceId))
                .ReturnsAsync(Response.Success<Resource>(resource));

            _messageRepoMock.Setup(x => x.TryAddAsync(It.IsAny<PrivateMessage>()))
                .ReturnsAsync((PrivateMessage m) => Response.Success(m));

            // ACT
            var result = await _service.TrySendAsync(senderId, receiverId, content, resourceId);

            // ASSERT
            bool successCalled = false;
            string? errorMessage = null;
            PrivateMessage? sentMessage = null;

            result.OnSuccess(msg =>
            {
                successCalled = true;
                sentMessage = msg;
            })
            .OnFailure(err => errorMessage = err.Message);

            Assert.True(successCalled);
            Assert.Null(errorMessage);
            Assert.NotNull(sentMessage);
            Assert.Equal(senderId, sentMessage!.SentBy.Id);
            Assert.Equal(receiverId, sentMessage.SentTo.Id);
            Assert.Equal(content, sentMessage.Content);
            Assert.NotNull(sentMessage.QuotedResource);
            Assert.Equal(resourceId, sentMessage.QuotedResource!.Id);
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
            Assert.Equal("2", msgs.First().Content);
            Assert.Equal("1", msgs.Last().Content);
        }
    }
}