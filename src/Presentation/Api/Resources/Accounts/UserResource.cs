using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Controllers;
using ReSR.Presentation.Api.Resources.Resources;

namespace ReSR.Presentation.Api.Resources.Accounts;
public class UserResource(User from) : IResource<UserResource, User> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string UserName    { get; } = from.Username;
        public string Email       { get; } = from.Email;
        public string Permissions { get; } = from.Permissions.ToString();
        public bool Suspended     { get; } = from.Suspended;
        public bool IsAnonymous   { get; } = from.IsAnonymous;
        
        public UserLinks Links { get; } = new(
            Self           : GetLink(from),
            Anonymize      : GetLink(from).WithSubRoute("anonymize").WithMethod(HttpMethod.POST),
            Friends        : GetLinks(from.Friends),
            LikedUsers     : GetLinks(from.LikedUsers),
            Bookmarks      : ResourceResource.GetLinks(from.Bookmarks),
            OwnedResources : ResourceResource.GetLinks(from.OwnedResources)
        );

        public readonly record struct UserLinks(
            ILink Self,
            ILink Anonymize,
            IEnumerable<ILink> Friends,
            IEnumerable<ILink> LikedUsers,
            IEnumerable<ILink> Bookmarks,
            IEnumerable<ILink> OwnedResources
        );

    #endregion
    #region METHODS

        public static implicit operator UserResource(User from) => new (from);

        public static UserResource From(User from) => from;
        public static IEnumerable<UserResource> From(IEnumerable<User> from) => from.Select(From);

        public static ILink GetLink(User from) => new AnnotatedLink(HttpMethod.GET, from.Username, UserController.ROUTE, from.Id);
        public static IEnumerable<ILink> GetLinks(IEnumerable<User> from) => from.Select(GetLink);
        
    #endregion

}