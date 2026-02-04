using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Core;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class AccountRepository<T>(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : Repository<T>(dbContext, domainEventDispatcher) where T : Account<T> {

    protected override Task<IResponse<T>> TryValidateAsync(T entity) =>
        base.TryValidateAsync(entity)
            .OnSuccessAsync(async _ => !await this.AnyAsync(x => x.Id != entity.Id && x.Email == entity.Email)
                ? Response.Success()
                : Response.Failure(new InvariantException($"Il ne peut y avoir plusieurs comptes avec l'adresse mail '{entity.Email}' !")))
            .OnSuccessAsync(() => entity);

}