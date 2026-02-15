using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Resources;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
public class TextResourceController(
    IRepository<TextResource> repository,
    ITextResourceCommandService commandService
) : ControllerBase {

    public const string ROUTE = "/manage/text-resources";
    #region DTOS

        public readonly record struct CreateTextResourceDto(
            string Title,
            Id     CategoryId,
            string Relationships,
            string Content
        );

        public readonly record struct UpdateTextResourceDto(
            string? Title         = null,
            Id?     CategoryId    = null,
            string? Relationships = null,
            string? Content       = null
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the text resources.")]
        public Task<IResult> GetCategoriesAsync() =>
            repository.GetAllAsync().ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpGet("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the text resource.")]
        public Task<IResult> GetCategoryAsync(Id resourceId) =>
            repository.TryGetAsync(resourceId).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPost]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Creates a new text resource.")]
        public Task<IResult> PostCategoryAsync(CreateTextResourceDto dto) =>
            commandService
                .TryCreateAsync(dto.Title, dto.CategoryId, Enum.TryParse<Relationships>(dto.Relationships, true, out var permissions) ? permissions : Relationships.All, dto.Content)
                .ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPatch("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Updates the text resource.")]
        public Task<IResult> PatchCategoryAsync(Id resourceId, UpdateTextResourceDto dto) =>
            commandService
                .TryUpdateAsync(resourceId, dto.Title, dto.CategoryId, Enum.TryParse<Relationships>(dto.Relationships, true, out var permissions) ? permissions : null, dto.Content)
                .ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpDelete("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent), AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to delete the text resource.")]
        public Task<IResult> DeleteCategoryAsync(Id resourceId) =>
            commandService
                .TryDeleteAsync(resourceId)
                .ToResultAsync(Results.Ok);

    #endregion
    
}
