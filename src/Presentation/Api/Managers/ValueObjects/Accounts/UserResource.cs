using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Managers.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Domain.Extensions;

namespace ReSR.Presentation.Api.Managers.ValueObjects.Accounts;
public class UserResource(User from) : IResource<UserResource, User> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string              Username             { get; } = from.Username;
        public string              Email                { get; } = from.Email;
        public string              Permissions          { get; } = string.Join(',', from.Permissions.GetUniqueValues());
        public IEnumerable<string> LocalizedPermissions { get; } = from.Permissions.ToLocalizedNames();
        public bool                Suspended            { get; } = from.Suspended;
        public bool                IsAnonymous          { get; } = from.IsAnonymous;
        
        public UserLinks Links { get; } = new(
            Self      : GetLink(from),
            Anonymize : GetLink(from).WithSubRoute("anonymize").WithMethod(Core.ValueObjects.HttpMethod.POST),
            Suspend   : GetLink(from).WithSubRoute("suspend").WithMethod(Core.ValueObjects.HttpMethod.POST)
        );

        public readonly record struct UserLinks(
            AnnotatedLink Self,
            Link          Anonymize,
            Link          Suspend
        );

    #endregion
    #region METHODS

        public static implicit operator UserResource(User from) => new (from);

        public static UserResource From(User from) => from;
        public static IEnumerable<UserResource> From(IEnumerable<User> from) => from.Select(From);

        public static AnnotatedLink GetLink(User from) => new(Core.ValueObjects.HttpMethod.GET, from.Username, UserController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<User> from) => from.Select(GetLink);
        
    #endregion

}