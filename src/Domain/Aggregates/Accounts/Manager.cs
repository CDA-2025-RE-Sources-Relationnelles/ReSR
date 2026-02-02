using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Core;
using ReSR.Domain.Extensions;

namespace ReSR.Domain.Aggregates.Accounts;
public sealed record Manager : Account<Manager>, IAggregateRoot<Manager> {

    #region PROPERTIES

        /// <summary> The manager's CRUD permissions. </summary>
        public ManagerPermissions Permissions { get; internal init; }

    #endregion
    #region CONSTRUCTORS

        public static IResponse<Manager> TryCreate(
            string             email,
            string             password,
            ManagerPermissions permissions = ManagerPermissions.AdminRole
        ) => TryVerifyEmailInvariant(email)
                .OnSuccess(() => Password.TryCreate(password))
                .OnSuccess(password => new Manager {
                    Email        = email,
                    Password     = password,
                    Permissions  = permissions,
                    DomainEvents = [new AccountCreated<Manager>(email)]
                });

    #endregion
    #region METHODS

        /// <returns> A copy of the manager account with the given permissions. </returns>
        public Manager WithPermissions(ManagerPermissions value) =>
            this with { Permissions = value };

        /// <returns> A successful response if the permissions matches the manager's. </returns>
        public IResponse TryVerifyPermissions(ManagerPermissions value) =>
            this.Permissions.HasFlag(value)
            ? Response.Success()
            : Response.Failure($"Le manager n'a pas les permissions suivantes '{value.GetUniqueValues()}'");

    #endregion
    
}