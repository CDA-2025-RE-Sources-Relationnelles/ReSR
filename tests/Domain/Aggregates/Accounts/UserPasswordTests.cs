using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class UserPasswordTests {

    [Fact]
    public void TryWithPassword_WithValidPassword_ShouldSucceed() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var response = user.TryWithPassword("bbbbCCCC2222");

        // Assert
        Assert.IsType<ISuccess<User>>(response, exactMatch: false);
        Assert.NotEqual(user.Password, response.Unwrap().Password);
    }

    [Fact]
    public void TryWithPassword_WithUnsafePassword_ShouldFail() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var response = user.TryWithPassword("unsafePassword");

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPassword_WithCorrectPassword_ShouldSucceed() {

        // Arrange
        var user = UserTestFactory.CreateValidUser(password: "abcdABCD1234");

        // Act
        var response = user.TryVerifyPassword("abcdABCD1234");

        // Assert
        Assert.IsType<ISuccess>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPassword_WithWrongPassword_ShouldFail()  {

        // Arrange
        var user = UserTestFactory.CreateValidUser(password: "abcdABCD1234");

        // Act
        var response = user.TryVerifyPassword("wrongPassword");

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
