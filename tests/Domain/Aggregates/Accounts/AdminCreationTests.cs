using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class AdminCreationTests {
    
    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed()  {

        // Arrange
        var email    = "admin@test.com";
        var password = "abcdABCD1234";

        // Act
        var response = Admin.TryCreate(email, password);

        // Assert
        Assert.IsType<ISuccess<Admin>>(response, exactMatch: false);

        var user = response.Unwrap();
        Assert.Equal(email, user.Email);
        Assert.Single(user.DomainEvents);
        Assert.Contains(
            user.DomainEvents,
            e => e is AccountCreated<Admin>
        );
    }

    [Fact]
    public void TryCreate_WithInvalidEmail_ShouldFail() {

        // Arrange
        var email    = "notanemail";
        var password = "abcdABCD1234";

        // Act
        var response = Admin.TryCreate(email, password);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
