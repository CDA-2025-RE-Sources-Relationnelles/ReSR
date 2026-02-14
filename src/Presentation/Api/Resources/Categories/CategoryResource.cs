using System.Text.Json.Serialization;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Presentation.Api.Controllers;
using ReSR.Presentation.Api.Resources.Resources;

namespace ReSR.Presentation.Api.Resources.Categories;
public class CategoryResource(Category resource) : IResource<CategoryResource, Category> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = resource.Id;

        public string Name { get; } = resource.Name;
        
        public CategoryLinks Links { get; } = new(
            Self      : GetLink(resource),
            Resources : ResourceResource.GetLinks(resource.Resources)
        );

        public readonly record struct CategoryLinks(
            AnnotatedLink Self,
            IEnumerable<AnnotatedLink> Resources
        );

    #endregion
    #region METHODS

        public static implicit operator CategoryResource(Category from) => new (from);

        public static CategoryResource From(Category from) => from;
        public static IEnumerable<CategoryResource> From(IEnumerable<Category> from) => from.Select(From);

        public static AnnotatedLink GetLink(Category from) => new (HttpMethod.GET, from.Name, CategoryController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<Category> from) => from.Select(GetLink);
        
    #endregion

}