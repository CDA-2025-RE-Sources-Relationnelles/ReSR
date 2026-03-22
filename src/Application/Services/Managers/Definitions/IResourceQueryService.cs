using FluentResponse.Interfaces;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Managers.Definitions;
public interface IResourceQueryService<T> where T : Resource {
    public Task<IEnumerable<T>> GetAllAsync(
        string?       titleSearch         = null,
        Id?           categoryIdFilter    = null,
        Relationships relationshipsFilter = Relationships.None,
        OrderBy       orderBy             = OrderBy.Newest
    );

    public Task<IResponse<T>> TryGetAsync(Id id);
}
