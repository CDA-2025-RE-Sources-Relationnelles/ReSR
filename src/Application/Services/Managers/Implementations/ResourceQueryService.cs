using FluentResponse.Interfaces;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
public class ResourceQueryService<T>(
    IRepository<T> resourceRepository
) : IResourceQueryService<T> where T : Resource, IAggregateRoot<T> {

    public async Task<IEnumerable<T>> GetAllAsync(
        Id? categoryIdFilter,
        Relationships relationshipsFilter,
        OrderBy       orderBy
    ) {
        var resources = await resourceRepository.GetAllAsync(x =>
            x.Visibility == Visibility.Public &&
            (categoryIdFilter == null || x.Category.Id == categoryIdFilter) &&
            (x.Relationships & relationshipsFilter) == relationshipsFilter
        );

        return orderBy switch {
            OrderBy.Newest    => resources.OrderByDescending(x => x.EditedAt),
            OrderBy.Oldest    => resources.OrderBy(x => x.EditedAt),
            OrderBy.LikeCount => resources.OrderByDescending(x => x.LikeCount),
            _ => resources
        };
    }

    public Task<IResponse<T>> TryGetAsync(Id id) =>
        resourceRepository.TryGetAsync(id);
}