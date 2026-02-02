using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;

internal static class UserTestFactory {
    public static User CreateValidUser(
        string          username    = "User",
        string          email       = "test@test.com",
        string          password    = "abcdABCD1234",
        UserPermissions permissions = UserPermissions.None
    ) {
        var response = User.TryCreate(username, email, password, permissions);
        Assert.IsType<ISuccess<User>>(response, exactMatch: false);
        return response.Unwrap();
    }
}
