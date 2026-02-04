using ReSR.Application.Services;
using ReSR.Domain.Aggregates.Accounts;
using FluentResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ReSR.Domain.Core;
using FluentResponse.Interfaces;

namespace ReSR.Application.Tests.Services;
public class UserAnonymizationServiceTests {

    #region PROPERTIES

        private readonly Mock<IServiceProvider> serviceProvider;
        private readonly Mock<IServiceScopeFactory> serviceScopeFactory;
        private readonly Mock<IServiceScope> serviceScope;
        private readonly Mock<IRepository<User>> userRepository;
        private readonly Mock<ILogger<UserAnonymizationService>> logger;

        private readonly UserAnonymizationService service;

        public UserAnonymizationServiceTests() {
            this.serviceProvider     = new();
            this.serviceScopeFactory = new();
            this.serviceScope        = new();
            this.userRepository      = new();
            this.logger              = new();

            this.serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(this.serviceScopeFactory.Object);

            this.serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(this.serviceScope.Object);

            this.serviceScope
                .SetupGet(x => x.ServiceProvider)
                .Returns(Mock.Of<IServiceProvider>(x =>
                    x.GetService(typeof(IRepository<User>)) == this.userRepository.Object
                ));

            this.service = new(
                this.serviceProvider.Object,
                this.logger.Object
            );
        }

    #endregion
    #region HandleAsync

    [Fact]
        public async Task HandleAsync_Should_Not_Start_Anonymization_When_Less_Than_35_Months_Inactive_And_Not_Started() {

            // Arrange
            var now = DateTime.UtcNow;
            var lastActivity = now.AddMonths(-34);
            var username = "username";
            var email = "nom@domaine.fr";
            var password = "abcdABCD1234";

            var user = User.TryCreate(username, email, password).Unwrap() with {
                LastActivity = lastActivity,
            };

            var updatedUser = user with { AnonymizationProcessStartedAt = now };

            var users = new List<User> { user };
            userRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            await service.HandleAsync();

            // Assert
            userRepository.Verify(x =>
                x.GetAllAsync(),
                Times.AtLeastOnce
            );

            userRepository.Verify(x =>
                x.TryUpdateAsync(
                    user.Id,
                    It.IsAny<Func<User, IResponse<User>>>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task HandleAsync_Should_Start_Anonymization_When_35_Months_Inactive_And_Not_Started() {

            // Arrange
            var now = DateTime.UtcNow;
            var lastActivity = now.AddMonths(-35).AddDays(-1);
            var username = "username";
            var email = "nom@domaine.fr";
            var password = "abcdABCD1234";

            var user = User.TryCreate(username, email, password).Unwrap() with {
                LastActivity = lastActivity,
            };

            var updatedUser = user with { AnonymizationProcessStartedAt = now };

            var users = new List<User> { user };
            userRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            userRepository
                .Setup(x => x.TryUpdateAsync(user.Id, It.IsAny<Func<User, IResponse<User>>>()))
                .ReturnsAsync(Response.Success(updatedUser));

            // Act
            await service.HandleAsync();

            // Assert
            userRepository.Verify(x =>
                x.GetAllAsync(),
                Times.AtLeastOnce
            );

            userRepository.Verify(x =>
                x.TryUpdateAsync(
                    user.Id,
                    It.IsAny<Func<User, IResponse<User>>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task HandleAsync_Should_Anonymize_When_Started_And_One_Month_Passed() {

            // Arrange
            var now = DateTime.UtcNow;
            var lastActivity = now.AddMonths(-36).AddDays(-1);
            var username = "username";
            var email = "nom@domaine.fr";
            var password = "abcdABCD1234";

            var user = User.TryCreate(username, email, password).Unwrap() with {
                LastActivity = lastActivity,
            };

            var updatedUser = user.AsAnonymized();

            var users = new List<User> { user };

            userRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            userRepository
                .Setup(x => x.TryUpdateAsync(user.Id, It.IsAny<Func<User, User>>()))
                .ReturnsAsync(Response.Success(updatedUser));

            // Act
            await service.HandleAsync();

            // Assert
            userRepository.Verify(x =>
                x.GetAllAsync(),
                Times.AtLeastOnce
            );

            userRepository.Verify(x =>
                x.TryUpdateAsync(
                    user.Id,
                    It.IsAny<Func<User, IResponse<User>>>()),
                Times.Once
            );
        }

    #endregion

}
