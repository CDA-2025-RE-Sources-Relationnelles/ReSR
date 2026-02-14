using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Presentation.Api.Resources.Accounts;
public class UserSessionResource(Session<User> from) : UserResource(from.Details), IResource<UserSessionResource, Session<User>> {

    #region PROPERTIES

        public string Token { get; } = from.Token;

    #endregion
    #region METHODS

        public static implicit operator UserSessionResource(Session<User> from) => new (from);

        public static UserSessionResource From(Session<User> from) => from;
        public static IEnumerable<UserSessionResource> From(IEnumerable<Session<User>> from) => from.Select(From);

        public static ILink GetLink(Session<User> from) => GetLink(from.Details);
        public static IEnumerable<ILink> GetLinks(IEnumerable<Session<User>> from) => from.Select(GetLink);

    #endregion

}