using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;
using ReSR.Domain.Ports;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class ResourceRepository<T>(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : Repository<T>(dbContext, domainEventDispatcher), IResourceRepository<T> where T : Resource, IAggregateRoot<T> {
    
    public async Task<IEnumerable<T>> GetAllAsync(
        string? titleSearch,
        Id? categoryIdFilter,
        Relationships relationshipsFilter,
        Visibility visibilityFilter
    ) => await this.GetJoinedTable().Where(x =>
        EF.Functions.ILike(x.Title, $"%{titleSearch}%") &&
        visibilityFilter.HasFlag(x.Visibility) &&
        (categoryIdFilter == null || x.Category.Id == categoryIdFilter) &&
        (x.Relationships & relationshipsFilter) == relationshipsFilter
    ).ToListAsync();

    protected override IQueryable<T> GetJoinedTable() =>
        base.GetJoinedTable()
            .Include(x => x.Category)
            .Include(x => x.Owner)
            .Include(x => x.LikedBy)
            .Include(x => x.BookmarkedBy)
            .Include(x => x.ExploitedBy)
            .Include(x => x.Comments);

    protected override Task<IResponse<T>> TryValidateAsync(T entity) =>
        base.TryValidateAsync(entity)
            .OnSuccessAsync(async _ => !await this.dbContext.Set<Resource>().AnyAsync(x => x.Id != entity.Id && x.Title == entity.Title && x.Category.Id == entity.Category.Id)
                ? Response.Success()
                : Response.Failure(new InvariantException($"Il ne peut y avoir plusieurs ressources avec le titre '{entity.Title}' dans la catégorie '{entity.Category.Name}' !")))
            .OnSuccessAsync(() => entity);

}