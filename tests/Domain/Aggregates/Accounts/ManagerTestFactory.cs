using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;

internal static class ManagerTestFactory {
    public static Manager CreateValidManager(
        string             email       = "manager@test.com",
        string             password    = "abcdABCD1234",
        ManagerPermissions permissions = ManagerPermissions.AdminRole
    ) {
        var response = Manager.TryCreate(email, password, permissions);
        Assert.IsType<ISuccess<Manager>>(response, exactMatch: false);
        return response.Unwrap();
    }
}
