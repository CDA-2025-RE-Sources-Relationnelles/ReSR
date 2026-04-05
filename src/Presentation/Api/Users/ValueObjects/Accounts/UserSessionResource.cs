using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Core.ValueObjects;

namespace ReSR.Presentation.Api.Users.ValueObjects.Accounts;
public class UserSessionResource(Session<User> from) : UserPrivateResource(from.Details), IResource<UserSessionResource, Session<User>> {

    #region PROPERTIES

        public string Token { get; } = from.Token;

    #endregion
    #region METHODS

        public static implicit operator UserSessionResource(Session<User> from) => new (from);

        public static UserSessionResource From(Session<User> from) => from;
        public static IEnumerable<UserSessionResource> From(IEnumerable<Session<User>> from) => from.Select(From);

    #endregion

}