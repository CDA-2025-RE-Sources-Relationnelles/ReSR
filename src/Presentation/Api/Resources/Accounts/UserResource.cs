using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Controllers;
using ReSR.Presentation.Api.Resources.Resources;

namespace ReSR.Presentation.Api.Resources.Accounts;
public class UserResource(User from) : IResource<UserResource, User> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string Username    { get; } = from.Username;
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
            AnnotatedLink Self,
            Link          Anonymize,
            IEnumerable<AnnotatedLink> Friends,
            IEnumerable<AnnotatedLink> LikedUsers,
            IEnumerable<AnnotatedLink> Bookmarks,
            IEnumerable<AnnotatedLink> OwnedResources
        );

    #endregion
    #region METHODS

        public static implicit operator UserResource(User from) => new (from);

        public static UserResource From(User from) => from;
        public static IEnumerable<UserResource> From(IEnumerable<User> from) => from.Select(From);

        public static AnnotatedLink GetLink(User from) => new(HttpMethod.GET, from.Username, UserController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<User> from) => from.Select(GetLink);
        
    #endregion

}