using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Controllers;

namespace ReSR.Presentation.Api.Resources.Accounts;
public class ManagerResource(Manager from) : IResource<ManagerResource, Manager> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string Email       { get; } = from.Email;
        public string Permissions { get; } = from.Permissions.ToString();
        
        public ManagerLinks Links { get; } = new(
            Self : GetLink(from)
        );

        public readonly record struct ManagerLinks(
            ILink Self
        );

    #endregion
    #region METHODS

        public static implicit operator ManagerResource(Manager from) => new (from);

        public static ManagerResource From(Manager from) => from;
        public static IEnumerable<ManagerResource> From(IEnumerable<Manager> from) => from.Select(From);

        public static ILink GetLink(Manager from) => new AnnotatedLink(HttpMethod.GET, from.Email, ManagerController.ROUTE, from.Id);
        public static IEnumerable<ILink> GetLinks(IEnumerable<Manager> from) => from.Select(GetLink);
        
    #endregion

}