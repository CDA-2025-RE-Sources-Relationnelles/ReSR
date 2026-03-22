using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Resources;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
[Authorize(Policy = "BackOffice")]
public class TextResourceController(
    IResourceQueryService<TextResource> queryService,
    ITextResourceCommandService commandService
) : ControllerBase {

    public const string ROUTE = "/manage/text-resources";
    #region DTOS

        public readonly record struct CreateTextResourceDto(
            string Title,
            string Description,
            Id     CategoryId,
            string Relationships,
            string Content
        );

        public readonly record struct UpdateTextResourceDto(
            string? Title         = null,
            string? Description   = null,
            Id?     CategoryId    = null,
            string? Relationships = null,
            string? Content       = null
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the text resources.")]
        public Task<IResult> GetTextResourcesAsync(
            int     pageIndex           = 0,
            int     pageSize            = 10,
            string? titleSearch         = null,
            Id?     categoryIdFilter    = null,
            string  relationshipsFilter = nameof(Relationships.None),
            string  orderBy             = nameof(OrderBy.Newest)
        ) => queryService.GetAllAsync(
            titleSearch: titleSearch,
            categoryIdFilter: categoryIdFilter,
            relationshipsFilter: Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.None,
            orderBy: Enum.TryParse<OrderBy>(orderBy, true, out var orderByParsed) ? orderByParsed : OrderBy.Newest
        ).ToPageResourceAsync<TextResource, TextResourceResource>(pageIndex, pageSize);

        [HttpGet("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the text resource.")]
        public Task<IResult> GetTextResourceAsync(Id resourceId) =>
            queryService.TryGetAsync(resourceId).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPost]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Creates a new text resource.")]
        public Task<IResult> PostTextResourceAsync(CreateTextResourceDto dto) =>
            commandService
                .TryCreateAsync(
                    title: dto.Title,
                    description: dto.Description,
                    categoryId: dto.CategoryId,
                    relationships: Enum.TryParse<Relationships>(dto.Relationships, true, out var permissions) ? permissions : Relationships.All,
                    content: dto.Content
                ).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPatch("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Updates the text resource.")]
        public Task<IResult> PatchTextResourceAsync(Id resourceId, UpdateTextResourceDto dto) =>
            commandService
                .TryUpdateAsync(
                    id: resourceId,
                    title: dto.Title,
                    description: dto.Description,
                    categoryId: dto.CategoryId,
                    relationships: Enum.TryParse<Relationships>(dto.Relationships, true, out var permissions) ? permissions : null,
                    content: dto.Content
                ).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/suspend")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to suspend the text resource.")]
        public Task<IResult> SuspendUserAsync(Id resourceId, bool value = true) =>
            commandService.TrySuspendAsync(resourceId, value).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpDelete("{resourceId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteContent))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to delete the text resource.")]
        public Task<IResult> DeleteTextResourceAsync(Id resourceId) =>
            commandService
                .TryDeleteAsync(resourceId)
                .ToResultAsync(Results.Ok);

    #endregion
    
}
