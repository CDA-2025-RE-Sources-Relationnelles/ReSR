using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Users.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Domain.Extensions;

namespace ReSR.Presentation.Api.Users.ValueObjects.Accounts;
public class UserPublicResource(User from) : IResource<UserPublicResource, User> {

    #region PROPERTIES

        public Id Id { get; } = from.Id;

        public string              Username             { get; } = from.Username;
        public IEnumerable<string> LocalizedPermissions { get; } = from.Permissions.ToLocalizedNames();
        
        public UserLinks Links { get; } = new(
            Self            : GetLink(from),
            LikeProfile     : GetLink(from).WithSubRoute("like-profile").WithMethod(Core.ValueObjects.HttpMethod.POST),
            LikedByUsers    : GetLinks(from.LikedBy),
            OwnedResources  : GetLink(from).WithSubRoute("owned-resources"),
            PrivateMessages : GetLink(from).WithSubRoute("private-messages")
        );

        public readonly record struct UserLinks(
            AnnotatedLink              Self,
            Link                       LikeProfile,
            IEnumerable<AnnotatedLink> LikedByUsers,
            Link                       OwnedResources,
            Link                       PrivateMessages
        );

    #endregion
    #region METHODS

        public static implicit operator UserPublicResource(User from) => new (from);

        public static UserPublicResource From(User from) => from;
        public static IEnumerable<UserPublicResource> From(IEnumerable<User> from) => from.Select(From);

        public static AnnotatedLink GetLink(User from) => new(Core.ValueObjects.HttpMethod.GET, from.Username, UserController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<User> from) => from.Select(GetLink);
        
    #endregion

}