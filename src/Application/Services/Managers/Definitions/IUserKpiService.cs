using ReSR.Application.ValueObjects.Accounts;
using ReSR.Application.ValueObjects.Core;

namespace ReSR.Application.Services.Managers.Definitions;
public interface IUserKpiService {

    /// <returns>The performance indicators for users that were active in the given date range.</returns>
    /// <param name="lastActivityFilter">A date range used to filter users based on their last activity.</param>
    public Task<UserKpi> GetAsync(DateRange lastActivityFilter = DateRange.AllTime);

    /// <returns>The global report of performance indicators for users.</returns>
    public IAsyncEnumerable<UserKpi> GetReportAsync();
}
