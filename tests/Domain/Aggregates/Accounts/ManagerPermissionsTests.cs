using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class ManagerPermissionsTests {
    
    [Fact]
    public void TryVerifyPermissions_WithCorrectPermissions_ShouldSucceed() {

        // Arrange
        var manager = ManagerTestFactory.CreateValidManager();

        // Act
        var response = manager.TryVerifyPermissions(ManagerPermissions.ReadContent);

        // Assert
        Assert.IsType<ISuccess>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPermissions_WithCorrectRole_ShouldSucceed() {

        // Arrange
        var manager = ManagerTestFactory.CreateValidManager();

        // Act
        var response = manager.TryVerifyPermissions(ManagerPermissions.AdminRole);

        // Assert
        Assert.IsType<ISuccess>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPermissions_WithoutPermissions_ShouldFail() {

        // Arrange
        var manager = ManagerTestFactory.CreateValidManager(permissions: ManagerPermissions.None);

        // Act
        var response = manager.TryVerifyPermissions(ManagerPermissions.ReadContent);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
    
    [Fact]
    public void TryVerifyPermissions_WithoutRole_ShouldFail() {

        // Arrange
        var manager = ManagerTestFactory.CreateValidManager(permissions: ManagerPermissions.None);

        // Act
        var response = manager.TryVerifyPermissions(ManagerPermissions.AdminRole);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPermissions_WithoutAdequateRole_ShouldFail() {

        // Arrange
        var manager = ManagerTestFactory.CreateValidManager();

        // Act
        var response = manager.TryVerifyPermissions(ManagerPermissions.SuperAdminRole);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
