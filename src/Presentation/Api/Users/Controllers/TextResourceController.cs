using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Resources;
using ReSR.Presentation.Api.Users.Authorization;
using ReSR.Presentation.Api.Users.Extensions;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
public class TextResourceController(
    ITextResourceService resourceService,
    IRepository<TextResource> repository
) : ControllerBase {

    public const string ROUTE = "/text-resources";
    #region DTOS

        public readonly record struct CreateUserTextResourceDto(
            string Title,
            Id     CategoryId,
            string Relationships,
            string Content,
            bool   IsPrivate
        );

        public readonly record struct UpdateUserTextResourceDto(
            string? Title         = null,
            Id?     CategoryId    = null,
            string? Relationships = null,
            string? Content       = null
        );

        public readonly record struct PostTextResourceCommentDto(
            string Content
        );

    #endregion
    #region ROUTES

        [HttpGet("{resourceId}")]
        [Authorize(Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointDescription("Queries the text resource.")]
        public Task<IResult> GetTextResourceAsync(Id resourceId) =>
            repository.TryGetAsync(resourceId).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPost]
        [Authorize]
        [EndpointSummary("Only accessible for authenticated users.")]
        [EndpointDescription("Creates a new text resource.")]
        public Task<IResult> PostTextResourceAsync(CreateUserTextResourceDto dto) =>
            resourceService.TryCreateAsync(
                ownerId: User.GetUserId()!.Value,
                title: dto.Title,
                categoryId: dto.CategoryId,
                relationships: Enum.TryParse<Relationships>(dto.Relationships, true, out var relationships) ? relationships : Relationships.All,
                content: dto.Content,
                isPrivate: dto.IsPrivate
            ).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPatch("{resourceId}")]
        [Authorize(Policy = nameof(ResourceWriteAuthorizationRequirement))]
        [EndpointSummary("Only accessible for the resource owner.")]
        [EndpointDescription("Updates the text resource.")]
        public Task<IResult> PatchTextResourceAsync(Id resourceId, UpdateUserTextResourceDto dto) =>
            resourceService.TryUpdateAsync(
                id: resourceId,
                title: dto.Title,
                categoryId: dto.CategoryId,
                relationships: Enum.TryParse<Relationships>(dto.Relationships, true, out var relationships) ? relationships : null,
                content: dto.Content
            ).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpDelete("{resourceId}")]
        [Authorize(Policy = nameof(ResourceWriteAuthorizationRequirement))]
        [EndpointSummary("Only accessible for the resource owner.")]
        [EndpointDescription("Deletes the text resource.")]
        public Task<IResult> DeleteTextResourceAsync(Id resourceId) =>
            repository.TryDeleteAsync(resourceId).ToResultAsync(Results.Ok);

        [HttpPost("{resourceId}/comments")]
        [Authorize(Roles = nameof(User), Policy = nameof(ResourceReadAuthorizationRequirement))]
        [EndpointSummary("Only accessible for authenticated users with access to the resource.")]
        [EndpointDescription("Posts a comment to the text resource")]
        public Task<IResult> PostTextResourceComment(Id resourceId, PostTextResourceCommentDto dto) =>
            resourceService.TryPostComment(resourceId, User.GetUserId()!.Value, dto.Content).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/confirm-verification")]
        [Authorize(Roles = nameof(UserPermissions.VerifyResources))]
        [EndpointSummary("Only accessible for authenticated users with resource verification permissions.")]
        [EndpointDescription("Confirms the text resource verification.")]
        public Task<IResult> ConfirmTextResourceVerification(Id resourceId) =>
            resourceService.TryConfirmVerification(resourceId).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

        [HttpPost("{resourceId}/reject-verification")]
        [Authorize(Roles = nameof(UserPermissions.VerifyResources))]
        [EndpointSummary("Only accessible for authenticated users with resource verification permissions.")]
        [EndpointDescription("Cancels the text resource verification.")]
        public Task<IResult> RejectTextResourceVerification(Id resourceId) =>
            resourceService.TryRejectVerification(resourceId).ToResourceAsync<TextResource, TextResourceResource>(Results.Ok);

    #endregion
    
}
