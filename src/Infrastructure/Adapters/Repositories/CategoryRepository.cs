using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Core;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class CategoryRepository(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : Repository<Category>(dbContext, domainEventDispatcher) {

    protected override IQueryable<Category> GetJoinedTable() =>
        base.GetJoinedTable().Include(x => x.Resources);

    protected override Task<IResponse<Category>> TryValidateAsync(Category entity) =>
        base.TryValidateAsync(entity)
            .OnSuccessAsync(async _ => !await this.AnyAsync(x => x.Id != entity.Id && x.Name == entity.Name)
                ? Response.Success()
                : Response.Failure(new InvariantException($"Il ne peut y avoir plusieurs catégories avec le nom '{entity.Name}' !")))
            .OnSuccessAsync(() => entity);

}