using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class UserPermissionsTests {
    
    [Fact]
    public void TryVerifyPermissions_WithCorrectPermissions_ShouldSucceed() {

        // Arrange
        var user = UserTestFactory.CreateValidUser(
            permissions: UserPermissions.ModeratorRole
        );

        // Act
        var response = user.TryVerifyPermissions(UserPermissions.VerifyComments);

        // Assert
        Assert.IsType<ISuccess>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPermissions_WithCorrectRole_ShouldSucceed() {

        // Arrange
        var user = UserTestFactory.CreateValidUser(
            permissions: UserPermissions.ModeratorRole
        );

        // Act
        var response = user.TryVerifyPermissions(UserPermissions.ModeratorRole);

        // Assert
        Assert.IsType<ISuccess>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPermissions_WithoutPermissions_ShouldFail() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var response = user.TryVerifyPermissions(UserPermissions.VerifyComments);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
    
    [Fact]
    public void TryVerifyPermissions_WithoutRole_ShouldFail() {

        // Arrange
        var user = UserTestFactory.CreateValidUser();

        // Act
        var response = user.TryVerifyPermissions(UserPermissions.ModeratorRole);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
