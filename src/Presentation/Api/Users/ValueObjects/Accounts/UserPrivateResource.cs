using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Users.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Domain.Extensions;

namespace ReSR.Presentation.Api.Users.ValueObjects.Accounts;
public class UserPrivateResource(User from) : IResource<UserPrivateResource, User> {

    #region PROPERTIES

        public Id Id { get; } = from.Id;

        public string              Username             { get; } = from.Username;
        public string              Email                { get; } = from.Email;
        public string              Permissions          { get; } = string.Join(',', from.Permissions.GetUniqueValues());
        public IEnumerable<string> LocalizedPermissions { get; } = from.Permissions.ToLocalizedNames();
        public int                 OwnedResourceCount   { get; } = from.OwnedResources.Count;
        public int                 LikeCount            { get; } = from.Likes.Count;
        public int                 BookmarkCount        { get; } = from.Bookmarks.Count;
        public int                 ExploitCount         { get; } = from.Exploits.Count;
        
        public UserLinks Links { get; } = new(
            Self                : GetLink(from),
            RefreshSession      : GetLink(from).WithSubRoute("refresh-session").WithMethod(Core.ValueObjects.HttpMethod.POST),
            Anonymize           : GetLink(from).WithSubRoute("anonymize").WithMethod(Core.ValueObjects.HttpMethod.POST),
            LikeProfile         : GetLink(from).WithSubRoute("like-profile").WithMethod(Core.ValueObjects.HttpMethod.POST),
            LikedByUsers        : GetLinks(from.LikedBy),
            Friends             : GetLinks(from.Friends),
            OwnedResources      : GetLink(from).WithSubRoute("owned-resources"),
            BookmarkedResources : GetLink(from).WithSubRoute("bookmarked-resources")
        );

        public readonly record struct UserLinks(
            AnnotatedLink Self,
            Link          RefreshSession,
            Link          Anonymize,
            Link          LikeProfile,
            IEnumerable<AnnotatedLink> LikedByUsers,
            IEnumerable<AnnotatedLink> Friends,
            Link OwnedResources,
            Link BookmarkedResources
        );

    #endregion
    #region METHODS

        public static implicit operator UserPrivateResource(User from) => new (from);

        public static UserPrivateResource From(User from) => from;
        public static IEnumerable<UserPrivateResource> From(IEnumerable<User> from) => from.Select(From);

        public static AnnotatedLink GetLink(User from) => new(Core.ValueObjects.HttpMethod.GET, from.Username, UserController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<User> from) => from.Select(GetLink);
        
    #endregion

}