using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class ManagerEmailTests {

    [Fact]
    public void TryWithMailAddress_WithValidEmail_ShouldUpdateAndRaiseEvent() {

        // Arrange
        var manager = ManagerTestFactory.CreateValidManager();

        // Act
        var newEmail = "new@test.com";
        var response = manager.TryWithMailAddress(newEmail);

        // Assert
        Assert.IsType<ISuccess<Manager>>(response, exactMatch: false);

        var updated = response.Unwrap();
        Assert.Equal(newEmail, updated.Email);
        Assert.Contains(
            updated.DomainEvents,
            e => e is AccountEmailChanged<Manager>
        );
    }

    [Fact]
    public void TryWithMailAddress_WithSameEmail_ShouldNotRaiseEvent() {

        // Arrange
        var manager = ManagerTestFactory.CreateValidManager();

        // Act
        var response = manager.TryWithMailAddress(manager.Email);

        // Assert
        Assert.IsType<ISuccess<Manager>>(response, exactMatch: false);
        Assert.Same(manager, response.Unwrap());
    }

    [Fact]
    public void TryWithMailAddress_WithInvalidEmail_ShouldFail() {

        // Arrange
        var manager = ManagerTestFactory.CreateValidManager();

        // Act
        var response = manager.TryWithMailAddress("invalid");

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
