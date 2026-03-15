using System.Text.Json.Serialization;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Extensions;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Presentation.Api.Users.ValueObjects.Accounts;
using ReSR.Presentation.Api.Users.ValueObjects.Categories;

namespace ReSR.Presentation.Api.Users.ValueObjects.Resources;
public abstract class ResourceResource(Resource from) : IResource<ResourceResource, Resource> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string              Title                  { get; } = from.Title;
        public string              Description            { get; } = from.Description;
        public string              Relationships          { get; } = from.Relationships.ToString();
        public IEnumerable<string> LocalizedRelationships { get; } = from.Relationships.ToLocalizedNames();
        public string              LocalizedVisibility    { get; } = from.Visibility.ToLocalizedName();
        public string              PublishedAt            { get; } = from.PublishedAt.ToString();
        public string              EditedAt               { get; } = from.EditedAt.ToString();
        public uint                LikeCount              { get; } = from.LikeCount;
        public uint                BookmarkCount          { get; } = from.BookmarkCount;
        public uint                ExploitCount           { get; } = from.ExploitCount;

        public bool? LikedBySession      { get; protected set; }
        public bool? ExploitedBySession  { get; protected set; }
        public bool? BookmarkedBySession { get; protected set; }

        public virtual ResourceLinks Links { get; } = new(
            Self     : GetLink(from),
            Category : CategoryResource.GetLink(from.Category),
            Owner    : from.Owner is not null ? UserPrivateResource.GetLink(from.Owner) : null,
            Comments : GetLink(from).WithSubRoute("comments"),
            Like     : GetLink(from).WithSubRoute("like").WithMethod(Core.ValueObjects.HttpMethod.POST),
            Bookmark : GetLink(from).WithSubRoute("bookmark").WithMethod(Core.ValueObjects.HttpMethod.POST),
            Exploit  : GetLink(from).WithSubRoute("exploit").WithMethod(Core.ValueObjects.HttpMethod.POST)
        );

        public readonly record struct ResourceLinks(
            AnnotatedLink  Self,
            AnnotatedLink  Category,
            Link           Comments,
            Link           Like,
            Link           Bookmark,
            Link           Exploit,
            AnnotatedLink? Owner
        );

    #endregion
    #region METHODS

        public T WithInjectedUserContext<T>(Id userId) where T : ResourceResource {
            this.LikedBySession      = from.LikedBy.Any(x => x.Id == userId);
            this.ExploitedBySession  = from.ExploitedBy.Any(x => x.Id == userId);
            this.BookmarkedBySession = from.BookmarkedBy.Any(x => x.Id == userId);
            return (T)this;
        }

        public static implicit operator ResourceResource(Resource from) => from switch {
            QuizResource fromActual => QuizResourceResource.From(fromActual),
            TextResource fromActual => TextResourceResource.From(fromActual),
            _ => throw new NotImplementedException()
        };

        public static ResourceResource From(Resource from) => from;
        public static IEnumerable<ResourceResource> From(IEnumerable<Resource> from) => from.Select(From);

        public static AnnotatedLink GetLink(Resource from) => from switch {
            QuizResource fromActual => QuizResourceResource.GetLink(fromActual),
            TextResource fromActual => TextResourceResource.GetLink(fromActual),
            _ => throw new NotImplementedException()
        };

        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<Resource> from) => from.Select(GetLink);
        
    #endregion

}