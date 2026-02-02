using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class AdminEmailTests {

    [Fact]
    public void TryWithMailAddress_WithValidEmail_ShouldUpdateAndRaiseEvent() {

        // Arrange
        var admin = AdminTestFactory.CreateValidAdmin();

        // Act
        var newEmail = "new@test.com";
        var response = admin.TryWithMailAddress(newEmail);

        // Assert
        Assert.IsType<ISuccess<Admin>>(response, exactMatch: false);

        var updated = response.Unwrap();
        Assert.Equal(newEmail, updated.Email);
        Assert.Contains(
            updated.DomainEvents,
            e => e is AccountEmailChanged<Admin>
        );
    }

    [Fact]
    public void TryWithMailAddress_WithSameEmail_ShouldNotRaiseEvent() {

        // Arrange
        var admin = AdminTestFactory.CreateValidAdmin();

        // Act
        var response = admin.TryWithMailAddress(admin.Email);

        // Assert
        Assert.IsType<ISuccess<Admin>>(response, exactMatch: false);
        Assert.Same(admin, response.Unwrap());
    }

    [Fact]
    public void TryWithMailAddress_WithInvalidEmail_ShouldFail() {

        // Arrange
        var admin = AdminTestFactory.CreateValidAdmin();

        // Act
        var response = admin.TryWithMailAddress("invalid");

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
