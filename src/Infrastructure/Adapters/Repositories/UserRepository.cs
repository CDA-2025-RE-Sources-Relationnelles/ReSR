using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Core;
using ReSR.Domain.Ports;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class UserRepository(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : AccountRepository<User>(dbContext, domainEventDispatcher) {

    protected override IQueryable<User> GetJoinedTable() =>
        base.GetJoinedTable()
            .Include(x => x.LikedUsers)
            .Include(x => x.LikedBy)
            .Include(x => x.Bookmarks)
            .Include(x => x.OwnedResources);
            
    protected override Task<IResponse<User>> TryValidateAsync(User entity) =>
        base.TryValidateAsync(entity)
            .OnSuccessAsync(async _ => !await this.AnyAsync(x => x.Id != entity.Id && x.Username == entity.Username)
                ? Response.Success()
                : Response.Failure(new InvariantException($"Il ne peut y avoir plusieurs utilisateurs avec l'identifiant '{entity.Username}' !")))
            .OnSuccessAsync(() => entity);

}