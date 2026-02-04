using ReSR.Application.Services;
using ReSR.Domain.Aggregates.Accounts;
using FluentResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ReSR.Domain.Core;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.Resources;
using System.Linq.Expressions;

namespace ReSR.Application.Tests.Services;
public class CommentReportForgivenessServiceTests {

    #region PROPERTIES

        private readonly Mock<IServiceProvider> serviceProvider;
        private readonly Mock<IServiceScopeFactory> serviceScopeFactory;
        private readonly Mock<IServiceScope> serviceScope;
        private readonly Mock<IRepository<Comment>> commentRepository;
        private readonly Mock<ILogger<CommentReportForgivenessService>> logger;

        private readonly CommentReportForgivenessService service;

        public CommentReportForgivenessServiceTests() {
            this.serviceProvider     = new();
            this.serviceScopeFactory = new();
            this.serviceScope        = new();
            this.commentRepository   = new();
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
                    x.GetService(typeof(IRepository<Comment>)) == this.commentRepository.Object
                ));

            this.service = new(
                this.serviceProvider.Object,
                this.logger.Object
            );
        }

    #endregion
    #region HandleAsync

    [Fact]
        public async Task HandleAsync_Should_Not_Forgive_Report_When_Less_Than_5_Months_Since_Report() {

            // Arrange
            var now = DateTime.UtcNow;
            var reportedAt = now.AddMonths(-5);
            var user = new Mock<User>();
            var resource = new Mock<TextResource>();

            var comment = Comment.TryCreate(user.Object, "Content", resource.Object).Unwrap() with {
                Reports = [new() { Content = "Content", ReportedBy = user.Object, ReportedAt = reportedAt }],
            };

            var comments = new List<Comment> { comment };
            commentRepository
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ReturnsAsync(comments);

            // Act
            await service.HandleAsync();

            // Assert
            commentRepository.Verify(x =>
                x.GetAllAsync(It.IsAny<Expression<Func<Comment, bool>>>()),
                Times.AtLeastOnce
            );

            commentRepository.Verify(x =>
                x.TryUpdateAsync(
                    comment.Id,
                    It.IsAny<Func<Comment, Comment>>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task HandleAsync_Should_Forgive_Report_Anonymization_When_6_Months_Since_Report() {

            // Arrange
            var now = DateTime.UtcNow;
            var reportedAt = now.AddMonths(-6).AddDays(-1);
            var user = new Mock<User>();
            var resource = new Mock<TextResource>();

            var comment = Comment.TryCreate(user.Object, "Content", resource.Object).Unwrap() with {
                Reports = [new() { Content = "Content", ReportedBy = user.Object, ReportedAt = reportedAt }],
            };

            var comments = new List<Comment> { comment };
            commentRepository
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<Comment, bool>>>()))
                .ReturnsAsync(comments);

            // Act
            await service.HandleAsync();

            // Assert
            commentRepository.Verify(x =>
                x.GetAllAsync(It.IsAny<Expression<Func<Comment, bool>>>()),
                Times.AtLeastOnce
            );

            commentRepository.Verify(x =>
                x.TryUpdateAsync(
                    comment.Id,
                    It.IsAny<Func<Comment, Comment>>()
                ),
                Times.Once
            );
        }

    #endregion

}
