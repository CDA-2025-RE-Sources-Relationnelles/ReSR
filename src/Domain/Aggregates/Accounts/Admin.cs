using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Core;
using ReSR.Domain.Extensions;

namespace ReSR.Domain.Aggregates.Accounts;
public sealed record Admin : Account<Admin>, IAggregateRoot<Admin> {

    #region PROPERTIES

        /// <summary> The admin's CRUD permissions. </summary>
        public AdminPermissions Permissions { get; internal init; }

    #endregion
    #region CONSTRUCTORS

        public static IResponse<Admin> TryCreate(
            string           email,
            string           password,
            AdminPermissions adminPermissions = AdminPermissions.AdminRole
        ) => TryVerifyEmailInvariant(email)
                .OnSuccess(() => Password.TryCreate(password))
                .OnSuccess(password => new Admin {
                    Email        = email,
                    Password     = password,
                    Permissions  = adminPermissions,
                    DomainEvents = [new AccountCreated<Admin>(email)]
                });

    #endregion
    #region METHODS

        /// <returns> A copy of the admin account with the given permissions. </returns>
        public Admin WithPermissions(AdminPermissions value) =>
            this with { Permissions = value };

        /// <returns> A successful response if the permissions matches the admin's. </returns>
        public IResponse TryVerifyPermissions(AdminPermissions value) =>
            this.Permissions.HasFlag(value)
            ? Response.Success()
            : Response.Failure($"L'administrateur n'a pas les permissions suivantes '{value.GetUniqueValues()}'");

    #endregion
    
}