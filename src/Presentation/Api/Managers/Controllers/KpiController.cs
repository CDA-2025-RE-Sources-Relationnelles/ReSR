using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Ports;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Application.ValueObjects.Core;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Presentation.Api.Core.Extensions;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
[Authorize(Policy = "BackOffice")]
public class KpiController(
    IUserKpiService userKpiService,
    IExportService<UserKpi> userKpiExportService,
    IResourceKpiService resourceKpiService,
    IExportService<ResourceKpi> resouceKpiExportService
) : ControllerBase {

    public const string ROUTE = "/manage/kpi";

    #region ROUTES

        [HttpGet(ROUTE + "/users/{dateRange}")]
        [EndpointDescription("Generates users KPI.")]
        public Task<IResult> GetUserKpiAsync(string dateRange) =>
            userKpiService
                .GetAsync(Enum.TryParse<DateRange>(dateRange, true, out var dateRangeParsed) ? dateRangeParsed : DateRange.AllTime)
                .ToResultAsync(Results.Ok);

        [HttpGet(ROUTE + "/users/download")]
        [EndpointDescription("Generates users KPI.")]
        public async Task<IResult> DownloadUserKpiAsync() =>
            userKpiExportService
                .Export(await userKpiService.GetReportAsync().ToListAsync())
                .ToResult(x => Results.File(x.Value, "text/csv", "user-kpi.csv"));

        [HttpGet(ROUTE + "/resources/{dateRange}")]
        [EndpointDescription("Generates resources KPI.")]
        public Task<IResult> GetResourceKpiAsync(
            string  dateRange,
            string? relationshipsFilter = null,
            string? visibilityFilter    = null,
            Id?     categoryIdFilter    = null
        ) =>
            resourceKpiService
                .GetAsync(
                    Enum.TryParse<DateRange>(dateRange, true, out var dateRangeParsed) ? dateRangeParsed : DateRange.AllTime,
                    Enum.TryParse<Relationships>(relationshipsFilter, true, out var relationshipsFilterParsed) ? relationshipsFilterParsed : Relationships.All,
                    Enum.TryParse<Visibility>(visibilityFilter, true, out var visibilityFilterParsed) ? visibilityFilterParsed : Visibility.Public,
                    categoryIdFilter
                ).ToResultAsync(Results.Ok);

        [HttpGet(ROUTE + "/resources/download")]
        [EndpointDescription("Generates resources KPI.")]
        public async Task<IResult> DownloadResourceKpiAsync() =>
            resouceKpiExportService
                .Export(await resourceKpiService.GetReportAsync().ToListAsync())
                .ToResult(x => Results.File(x.Value, "text/csv", "resource-kpi.csv"));

    #endregion
    
}
