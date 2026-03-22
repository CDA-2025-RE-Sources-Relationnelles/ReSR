using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Extensions;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Presentation.Api.Managers.ValueObjects.Accounts;
using ReSR.Presentation.Api.Managers.ValueObjects.Categories;

namespace ReSR.Presentation.Api.Managers.ValueObjects.Resources;
public abstract class ResourceResource(Resource from) : IResource<ResourceResource, Resource> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = from.Id;

        public string              Title                  { get; } = from.Title;
        public string              Type                   { get; } = from.GetType().Name;
        public string              Description            { get; } = from.Description;
        public string              Relationships          { get; } = string.Join(',', from.Relationships.GetUniqueValues());
        public string              Visibility             { get; } = from.Visibility.ToString();
        public IEnumerable<string> LocalizedRelationships { get; } = from.Relationships.ToLocalizedNames();
        public string              LocalizedVisibility    { get; } = from.Visibility.ToLocalizedName();
        public string              PublishedAt            { get; } = from.PublishedAt.ToString();
        public string              EditedAt               { get; } = from.EditedAt.ToString();
        public uint                LikeCount              { get; } = from.LikeCount;
        public uint                BookmarkCount          { get; } = from.BookmarkCount;
        public uint                ExploitCount           { get; } = from.ExploitCount;

        public ResourceLinks Links { get; } = new(
            Self     : GetLink(from),
            Category : CategoryResource.GetLink(from.Category),
            Owner    : from.Owner is not null ? UserResource.GetLink(from.Owner) : null,
            Suspend  : GetLink(from).WithSubRoute("suspend").WithMethod(Core.ValueObjects.HttpMethod.POST)
        );

        public readonly record struct ResourceLinks(
            AnnotatedLink  Self,
            AnnotatedLink  Category,
            AnnotatedLink? Owner,
            Link           Suspend
        );

    #endregion
    #region METHODS

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