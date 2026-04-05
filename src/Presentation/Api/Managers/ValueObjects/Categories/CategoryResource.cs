using ReSR.Domain.Aggregates.Categories;
using ReSR.Presentation.Api.Managers.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Presentation.Api.Managers.ValueObjects.Resources;

namespace ReSR.Presentation.Api.Managers.ValueObjects.Categories;
public class CategoryResource(Category resource) : IResource<CategoryResource, Category> {

    #region PROPERTIES

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

        public static AnnotatedLink GetLink(Category from) => new (Core.ValueObjects.HttpMethod.GET, from.Name, CategoryController.ROUTE, from.Id);
        public static IEnumerable<AnnotatedLink> GetLinks(IEnumerable<Category> from) => from.Select(GetLink);
        
    #endregion

}