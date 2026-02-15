using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Categories;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
public class CategoryController(
    IRepository<Category> repository,
    ICategoryCommandService commandService
) : ControllerBase {

    public const string ROUTE = "/manage/categories";
    #region DTOS

        public readonly record struct CreateOrUpdateCategoryDto(
            string Name
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the categories.")]
        public Task<IResult> GetCategoriesAsync() =>
            repository.GetAllAsync().ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpGet("{categoryId}")]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the category.")]
        public Task<IResult> GetCategoryAsync(Id categoryId) =>
            repository.TryGetAsync(categoryId).ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpPost]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Creates a new category.")]
        public Task<IResult> PostCategoryAsync(CreateOrUpdateCategoryDto dto) =>
            commandService
                .TryCreateAsync(dto.Name)
                .ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpPatch("{categoryId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Updates the category.")]
        public Task<IResult> PatchCategoryAsync(Id categoryId, CreateOrUpdateCategoryDto dto) =>
            commandService
                .TryUpdateAsync(categoryId, dto.Name)
                .ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to delete the category.")]
        public Task<IResult> DeleteCategoryAsync(Id categoryId) =>
            commandService.TryDeleteAsync(categoryId).ToResultAsync(Results.Ok);

    #endregion
    
}
