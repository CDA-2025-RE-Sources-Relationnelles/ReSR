using ReSR.Application.ValueObjects.Core;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Managers.Definitions;
public interface IResourceKpiService {

    /// <returns>The performance indicators for resources that were edited in the given date range.</returns>
    /// <param name="lastEditionFilter">A date range used to filter resources based on their last edition.</param>
    /// <param name="relationshipsFilter">Relationship type flags to filter resources based on their targeted relationship.</param>
    /// <param name="visibilityFilter">A visibility type used to filter resources based on their content visibility.</param>
    /// <param name="categoryIdFilter">A category type identifier used to filter resources based on their content category.</param>
    public Task<ResourceKpi> GetAsync(
        DateRange     lastEditionFilter   = DateRange.Any,
        Relationships relationshipsFilter = Relationships.All,
        Visibility    visibilityFilter    = Visibility.Public,
        Id?           categoryIdFilter    = null
    );
}
