using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Extensions;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Presentation.Api.Managers.Controllers;

namespace ReSR.Presentation.Api.Managers.ValueObjects.Accounts;
public class ManagerResource(Manager from) : IResource<ManagerResource, Manager> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string              Email                { get; } = from.Email;
        public string              Permissions          { get; } = from.Permissions.ToString();
        public IEnumerable<string> LocalizedPermissions { get; } = from.Permissions.ToLocalizedNames();
        
        public ManagerLinks Links { get; } = new(
            Self : GetLink(from)
        );

        public readonly record struct ManagerLinks(
            AnnotatedLink Self
        );

    #endregion
    #region METHODS

        public static implicit operator ManagerResource(Manager from) => new (from);

        public static ManagerResource From(Manager from) => from;
        public static IEnumerable<ManagerResource> From(IEnumerable<Manager> from) => from.Select(From);

        public static AnnotatedLink GetLink(Manager from) => new (Core.ValueObjects.HttpMethod.GET, from.Email, ManagerController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<Manager> from) => from.Select(GetLink);
        
    #endregion

}