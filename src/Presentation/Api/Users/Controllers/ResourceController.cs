using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Users.Extensions;
using ReSR.Presentation.Api.Users.ValueObjects.Resources;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
public class ResourceController(
    IResourceService<Resource> resourceService
) : ControllerBase {

    public const string ROUTE = "/resources";
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
    #region METHOD

    public static IResult WithInjectedUserContext<T>(ISuccess<T> success, Id? userId) where T : ResourceResource =>
        userId is Id id
            ? Results.Ok(success.OnSuccess(x => x.WithInjectedUserContext<T>(id)))
            : Results.Ok(success);

    #endregion
    #region ROUTES

        [HttpGet(ROUTE + "/public")]
        [EndpointDescription("Queries the public resources.")]
        public Task<IResult> GetPublicResourcesAsync(
            int     pageIndex           = 0,
            int     pageSize            = 10,
            string? titleSearch         = null,
            Id?     categoryIdFilter    = null,
            string  relationshipsFilter = nameof(Relationships.None),
            string  orderBy             = nameof(OrderBy.Newest)
        ) => resourceService.GetAllPublicAsync(
            titleSearch: titleSearch,
            categoryIdFilter: categoryIdFilter,
            relationshipsFilter: Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.None,
            orderBy: Enum.TryParse<OrderBy>(orderBy, true, out var orderByParsed) ? orderByParsed : OrderBy.Newest
        ).ToPageResourceAsync<Resource, ResourceResource>(
            pageIndex,
            pageSize,
            User.GetUserId() is Id userId ? (x) => x.WithInjectedUserContext<ResourceResource>(userId) : null
        );


        [HttpGet(ROUTE + "/private")]
        [Authorize]
        [EndpointSummary("Only accessible for authenticated users.")]
        [EndpointDescription("Queries the user's private resources.")]
        public Task<IResult> GetPrivateResourcesAsync(
            int     pageIndex           = 0,
            int     pageSize            = 10,
            string? titleSearch         = null,
            Id?     categoryIdFilter    = null,
            string  relationshipsFilter = nameof(Relationships.None),
            string  orderBy             = nameof(OrderBy.Newest)
        ) => resourceService.TryGetAllPrivateAsync(
            userId: User.GetUserId()!.Value,
            titleSearch: titleSearch,
            categoryIdFilter: categoryIdFilter,
            relationshipsFilter: Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.None,
            orderBy: Enum.TryParse<OrderBy>(orderBy, true, out var orderByParsed) ? orderByParsed : OrderBy.Newest
        ).ToPageResourceAsync<Resource, ResourceResource>(
            pageIndex,
            pageSize,
            User.GetUserId() is Id userId ? (x) => x.WithInjectedUserContext<ResourceResource>(userId) : null
        );

            
        [HttpGet(ROUTE + "/waiting-for-verification")]
        [EndpointSummary("Only accessible for authenticated users with resource verification permissions.")]
        [Authorize(Roles = nameof(UserPermissions.VerifyResources))]
        [EndpointDescription("Queries the resources waiting for verification.")]
        public Task<IResult> GetResourcesWaitingForVerificationAsync(
            int pageIndex = 0,
            int pageSize  = 10
        ) => resourceService
            .GetAllWaitingForVerificationAsync()
            .ToPageResourceAsync<Resource, ResourceResource>(
                pageIndex,
                pageSize,
                User.GetUserId() is Id userId ? (x) => x.WithInjectedUserContext<ResourceResource>(userId) : null
            );

    #endregion
    
}
