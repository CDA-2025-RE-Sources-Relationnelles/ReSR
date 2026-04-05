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
public class ResourceController(
    IResourceQueryService<Resource> queryService
) : ControllerBase {

    public const string ROUTE = "/manage/resources";

    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadContent))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the resources.")]
        public Task<IResult> GetResourcesAsync(
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
        ).ToPageResourceAsync<Resource, ResourceResource>(pageIndex, pageSize);

    #endregion
    
}
