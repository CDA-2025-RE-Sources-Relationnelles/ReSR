using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Resources;
using ReSR.Presentation.Api.Users.Extensions;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
public class ResourceController(
    IResourceService resourceService
) : ControllerBase {

    public const string ROUTE = "/resources";

    #region ROUTES

        [HttpGet(ROUTE + "/public")]
        [EndpointDescription("Queries the public resources.")]
        public Task<IResult> GetPublicResourcesAsync(
            string? relationshipsFilter = null,
            Id?     categoryIdFilter    = null
        ) => resourceService.GetAllPublic(
            categoryIdFilter: categoryIdFilter,
            relationshipsFilter: Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.All
        ).ToResourceAsync<Resource, ResourceResource>(Results.Ok);


        [HttpGet(ROUTE + "/private")]
        [Authorize(Policy = "FrontOffice")]
        [EndpointSummary("Only accessible for authenticated users.")]
        [EndpointDescription("Queries the user's private resources.")]
        public Task<IResult> GetPrivateResourcesAsync(
            string? relationshipsFilter = null,
            Id?     categoryIdFilter    = null
        ) => resourceService.TryGetAllPrivate(
            userId: User.GetUserId()!.Value,
            categoryIdFilter: categoryIdFilter,
            relationshipsFilter: Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.All
        ).ToResourceAsync<Resource, ResourceResource>(Results.Ok);

            
        [HttpGet(ROUTE + "/waiting-for-verification")]
        [EndpointSummary("Only accessible for authenticated users with resource verification permissions.")]
        [Authorize(Roles = nameof(UserPermissions.VerifyResources), Policy = "FrontOffice")]
        [EndpointDescription("Queries the resources waiting for verification.")]
        public Task<IResult> GetResourcesWaitingForVerificationAsync() =>
            resourceService
                .TryGetAllWaitingForVerification(User.GetUserId()!.Value)
                .ToResourceAsync<Resource, ResourceResource>(Results.Ok);

    #endregion
    
    
}
