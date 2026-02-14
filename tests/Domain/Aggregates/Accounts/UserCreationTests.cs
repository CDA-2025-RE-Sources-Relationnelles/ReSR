using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class UserCreationTests {
    
    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed()  {

        // Arrange
        var username = "user";
        var email    = "user@test.com";
        var password = "abcdABCD1234";

        // Act
        var response = User.TryCreate(username, email, password);

        // Assert
        Assert.IsType<ISuccess<User>>(response, exactMatch: false);

        var user = response.Unwrap();
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
        Assert.Single(user.DomainEvents);
        Assert.Contains(
            user.DomainEvents,
            e => e is AccountCreated<User>
        );
    }

    [Fact]
    public void TryCreate_WithInvalidEmail_ShouldFail() {

        // Arrange
        var username = "user";
        var email    = "notanemail";
        var password = "abcdABCD1234";

        // Act
        var response = User.TryCreate(username, email, password);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }

    [Fact]
    public void TryCreate_WithInvalidUsername_ShouldFail() {

        // Arrange
        var username = "é&sq";
        var email    = "user@test.com";
        var password = "abcdABCD1234";

        // Act
        var response = User.TryCreate(username, email, password);

        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
