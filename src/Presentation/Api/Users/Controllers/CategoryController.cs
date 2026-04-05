using Microsoft.AspNetCore.Mvc;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Categories;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
public class CategoryController(
    IRepository<Category> repository
) : ControllerBase {

    public const string ROUTE = "/categories";
    #region ROUTES

        [HttpGet]
        [EndpointDescription("Queries the categories.")]
        public Task<IResult> GetCategoriesAsync() =>
            repository.GetAllAsync().ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpGet("{categoryId}")]
        [EndpointDescription("Queries the category.")]
        public Task<IResult> GetCategoryAsync(Id categoryId) =>
            repository.TryGetAsync(categoryId).ToResourceAsync<Category, CategoryResource>(Results.Ok);

    #endregion
    
}
