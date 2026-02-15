using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Core.ValueObjects;

namespace ReSR.Presentation.Api.Managers.ValueObjects.Accounts;
public class ManagerSessionResource(Session<Manager> from) : ManagerResource(from.Details), IResource<ManagerSessionResource, Session<Manager>> {

    #region PROPERTIES

        public string Token { get; } = from.Token;

    #endregion
    #region METHODS

        public static implicit operator ManagerSessionResource(Session<Manager> from) => new (from);

        public static ManagerSessionResource From(Session<Manager> from) => from;
        public static IEnumerable<ManagerSessionResource> From(IEnumerable<Session<Manager>> from) => from.Select(From);

    #endregion

}