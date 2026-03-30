using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.EntityFrameworkCore;
using ReSR.Application.Exceptions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Core;
using ReSR.Domain.Ports;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class AccountRepository<T>(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : Repository<T>(dbContext, domainEventDispatcher), IAccountRepository<T> where T : Account<T> {

    public async Task<bool> AnyWithEmailAsync(string email) =>
        await this.table.ToAsyncEnumerable().AnyAsync(x => x.Email != "" && x.Email == email);

    public async Task<IResponse<T>> TryGetWithEmailAsync(string email) =>
        await this.GetJoinedTable().ToAsyncEnumerable().FirstOrDefaultAsync(x => x.Email == email) is T entity
            ? Response.Success(entity)
            : Response.Failure<T>(new EntityNotFoundException(typeof(T)));

    protected override Task<IResponse<T>> TryValidateAsync(T entity) =>
        base.TryValidateAsync(entity)
            .OnSuccessAsync(async _ => !await this.table.ToAsyncEnumerable().AnyAsync(x => x.Id != entity.Id && x.Email == entity.Email && x.Email != "")
                ? Response.Success()
                : Response.Failure(new InvariantException($"Il ne peut y avoir plusieurs comptes avec l'adresse mail '{entity.Email}' !")))
            .OnSuccessAsync(() => entity);

}