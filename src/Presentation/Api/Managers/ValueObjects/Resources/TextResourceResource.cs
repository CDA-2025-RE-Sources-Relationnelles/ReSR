using ReSR.Domain.Aggregates.Resources;
using ReSR.Presentation.Api.Users.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;

namespace ReSR.Presentation.Api.Managers.ValueObjects.Resources;
public class TextResourceResource(TextResource from) : ResourceResource(from), IResource<TextResourceResource, TextResource> {

    #region PROPERTIES

        public string Content { get; } = from.Content;

    #endregion
    #region METHODS

        public static implicit operator TextResourceResource(TextResource from) => new (from);

        public static TextResourceResource From(TextResource from) => from;
        public static IEnumerable<TextResourceResource> From(IEnumerable<TextResource> from) => from.Select(From);

        public static AnnotatedLink GetLink(TextResource from) => new (Core.ValueObjects.HttpMethod.GET, from.Title, TextResourceController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<TextResource> resources) => resources.Select(GetLink);
        
    #endregion

}