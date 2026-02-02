using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;

namespace ReSR.Domain.Tests.Aggregates.Accounts;
public class AdminPermissionsTests {
    
    [Fact]
    public void TryVerifyPermissions_WithCorrectPermissions_ShouldSucceed() {

        // Arrange
        var admin = AdminTestFactory.CreateValidAdmin();

        // Act
        var response = admin.TryVerifyPermissions(AdminPermissions.ViewCategoies);

        // Assert
        Assert.IsType<ISuccess>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPermissions_WithCorrectRole_ShouldSucceed() {

        // Arrange
        var admin = AdminTestFactory.CreateValidAdmin();

        // Act
        var response = admin.TryVerifyPermissions(AdminPermissions.AdminRole);

        // Assert
        Assert.IsType<ISuccess>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPermissions_WithoutPermissions_ShouldFail() {

        // Arrange
        var admin = AdminTestFactory.CreateValidAdmin(permissions: AdminPermissions.None);

        // Act
        var response = admin.TryVerifyPermissions(AdminPermissions.ViewCategoies    );

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
    
    [Fact]
    public void TryVerifyPermissions_WithoutRole_ShouldFail() {

        // Arrange
        var admin = AdminTestFactory.CreateValidAdmin(permissions: AdminPermissions.None);

        // Act
        var response = admin.TryVerifyPermissions(AdminPermissions.AdminRole);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }

    [Fact]
    public void TryVerifyPermissions_WithoutAdequateRole_ShouldFail() {

        // Arrange
        var admin = AdminTestFactory.CreateValidAdmin();

        // Act
        var response = admin.TryVerifyPermissions(AdminPermissions.SuperAdminRole);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
