using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Ports;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Application.ValueObjects.Core;
using ReSR.Presentation.Api.Core.Extensions;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
[Authorize(Policy = "BackOffice")]
public class KpiController(
    IUserKpiService userKpiService,
    IExportService<UserKpi> userKpiExportService
) : ControllerBase {

    public const string ROUTE = "/manage/kpi";

        #region ROUTES

        [HttpGet(ROUTE + "/users/{dateRange}")]
        [Authorize]
        [EndpointDescription("Generates users KPI.")]
        public Task<IResult> GetUserKpiAsync(string dateRange) =>
            userKpiService
                .GetAsync(Enum.TryParse<DateRange>(dateRange, true, out var permissions) ? permissions : DateRange.Any)
                .ToResultAsync(Results.Ok);

        [HttpGet(ROUTE + "/users/download")]
        [Authorize]
        [EndpointDescription("Generates users KPI.")]
        public async Task<IResult> DownloadUserKpiAsync() =>
            userKpiExportService.Export([
                await userKpiService.GetAsync(DateRange.Quarterly),
                await userKpiService.GetAsync(DateRange.Yearly),
                await userKpiService.GetAsync(DateRange.Any),
            ]).ToResult(x => Results.File(x.Value, "text/csv", "user-kpi.csv"));

    #endregion
    
}
