using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class UserStateTests {

    [Fact]
    public void WithSuspension_ShouldToggleAndRaiseEvent() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var suspended = user.WithSuspension();

        // Assert
        Assert.True(suspended.Suspended);
        Assert.Contains(
            suspended.DomainEvents,
            e => e is UserSuspensionChanged
        );
    }

    [Fact]
    public void AsAnonymized_ShouldClearEmailAndRaiseEvent() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var anonymized = user.AsAnonymized();

        // Assert
        Assert.True(anonymized.IsAnonymous);
        Assert.Equal(string.Empty, anonymized.Email);
        Assert.Contains(
            anonymized.DomainEvents,
            e => e is UserAnonymized
        );
    }

    [Fact]
    public void TryStartAnonymizationProcess_FirstTime_ShouldSucceed() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var response = user.TryWithNewAnonymizationProcess();

        // Assert
        Assert.IsType<ISuccess<User>>(response, exactMatch: false);
        Assert.NotNull(response.Unwrap().AnonymizationProcessStartedAt);
    }

    [Fact]
    public void TryStartAnonymizationProcess_AlreadyStarted_ShouldFail() {

        // Arrange
        var user = UserTestFactory
            .CreateValidUser()
            .TryWithNewAnonymizationProcess()
            .Unwrap();

        // Act
        var response = user.TryWithNewAnonymizationProcess();

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
