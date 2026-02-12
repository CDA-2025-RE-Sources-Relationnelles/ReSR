using ReSR.Domain.Aggregates.Resources;

namespace ReSR.Presentation.Api.Resources.Resources;
public class TextResourceResource(TextResource from) : ResourceResource(from), IResource<TextResourceResource, TextResource> {

    #region PROPERTIES

        public string Content { get; } = from.Content;

    #endregion
    #region METHODS

        public static implicit operator TextResourceResource(TextResource from) => new (from);

        public static TextResourceResource From(TextResource from) => from;
        public static IEnumerable<TextResourceResource> From(IEnumerable<TextResource> from) => from.Select(From);

        public static ILink GetLink(TextResource from) => new AnnotatedLink(HttpMethod.GET, from.Title, TextResourceController.ROUTE, from.Id);
        public static IEnumerable<ILink> GetLinks(IEnumerable<TextResource> resources) => resources.Select(GetLink);
        
    #endregion

}