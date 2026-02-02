using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;

internal static class AdminTestFactory {
    public static Admin CreateValidAdmin(
        string           email       = "admin@test.com",
        string           password    = "abcdABCD1234",
        AdminPermissions permissions = AdminPermissions.AdminRole
    ) {
        var response = Admin.TryCreate(email, password, permissions);
        Assert.IsType<ISuccess<Admin>>(response, exactMatch: false);
        return response.Unwrap();
    }
}
