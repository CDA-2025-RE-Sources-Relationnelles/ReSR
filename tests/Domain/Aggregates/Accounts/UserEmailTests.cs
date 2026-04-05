using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class UserEmailTests {

    [Fact]
    public void TryWithMailAddress_WithValidEmail_ShouldUpdateAndRaiseEvent() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var newEmail = "new@test.com";
        var response = user.TryWithMailAddress(newEmail);

        // Assert
        Assert.IsType<ISuccess<User>>(response, exactMatch: false);

        var updated = response.Unwrap();
        Assert.Equal(newEmail, updated.Email);
        Assert.Contains(
            updated.DomainEvents,
            e => e is AccountEmailChanged<User>
        );
    }

    [Fact]
    public void TryWithMailAddress_WithSameEmail_ShouldNotRaiseEvent() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var response = user.TryWithMailAddress(user.Email);

        // Assert
        Assert.IsType<ISuccess<User>>(response, exactMatch: false);
        Assert.Same(user, response.Unwrap());
    }

    [Fact]
    public void TryWithMailAddress_WithInvalidEmail_ShouldFail() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var response = user.TryWithMailAddress("invalid");

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
