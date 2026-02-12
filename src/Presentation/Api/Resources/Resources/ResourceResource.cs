using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Presentation.Api.Resources.Accounts;
using ReSR.Presentation.Api.Resources.Messages;

namespace ReSR.Presentation.Api.Resources.Resources;
public abstract class ResourceResource(Resource from) : IResource<ResourceResource, Resource> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string Title         { get; } = from.Title;
        public string Relationships { get; } = from.Relationships.ToString();
        public string Visibility    { get; } = from.Visibility.ToString();
        public string PublishedAt   { get; } = from.PublishedAt.ToString();
        public string EditedAt      { get; } = from.EditedAt.ToString();
        public uint   LikeCount     { get; } = from.LikeCount;
        public uint   BookmarkCount { get; } = from.BookmarkCount;
        public uint   ExploitCount  { get; } = from.ExploitCount;

        public IEnumerable<CommentResource> Comments { get; } = CommentResource.From(from.Comments.Where(x => x.AnsweredComment is null));

        public ResourceLinks Links { get; } = new(
            Self          : GetLink(from),
            Category      : CategoryResource.GetLink(from.Category),
            Owner         : from.Owner is not null ? UserResource.GetLink(from.Owner) : null,
            VerifyingUser : from.VerifyingUser is not null ? UserResource.GetLink(from.VerifyingUser) : null
        );

        public readonly record struct ResourceLinks(
            ILink  Self,
            ILink  Category,
            ILink? Owner,
            ILink? VerifyingUser
        );

    #endregion
    #region METHODS

        public static implicit operator ResourceResource(Resource from) => from switch {
            QuizResource fromActual => fromActual,
            TextResource fromActual => fromActual,
            _ => throw new NotImplementedException()
        };

        public static ResourceResource From(Resource from) => from;
        public static IEnumerable<ResourceResource> From(IEnumerable<Resource> from) => from.Select(From);

        public static ILink GetLink(Resource from) => from switch {
            QuizResource fromActual => QuizResourceResource.GetLink(fromActual),
            TextResource fromActual => TextResourceResource.GetLink(fromActual),
            _ => throw new NotImplementedException()
        };

        public static IEnumerable<ILink> GetLinks(IEnumerable<Resource> from) => from.Select(GetLink);
        
    #endregion

}