using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    IRepository<Category> repository
) : ControllerBase {

    public const string ROUTE = "/manage/categories";
    #region DTOS

        public readonly record struct CreateOrUpdateCategoryDto(
            string Name
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadCategories), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the categories.")]
        public Task<IResult> GetCategoriesAsync() =>
            repository.GetAllAsync().ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpGet("{categoryId}")]
        [Authorize(Roles = nameof(ManagerPermissions.ReadManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the category.")]
        public Task<IResult> GetCategoryAsync(Id categoryId) =>
            repository.TryGetAsync(categoryId).ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpPost]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Creates a new category.")]
        public Task<IResult> PostCategoryAsync(CreateOrUpdateCategoryDto dto) =>
            Category
                .TryCreate(dto.Name)
                .OnSuccessAsync(repository.TryAddAsync)
                .ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpPatch("{categoryId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Updates the category.")]
        public Task<IResult> PatchCategoryAsync(Id categoryId, CreateOrUpdateCategoryDto dto) =>
            repository
                .TryUpdateAsync(categoryId, manager => manager.TryWithName(dto.Name))
                .ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteManagers), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to delete the category.")]
        public Task<IResult> DeleteCategoryAsync(Id categoryId) =>
            repository.TryDeleteAsync(categoryId).ToResultAsync(Results.Ok);

    #endregion
    
}
