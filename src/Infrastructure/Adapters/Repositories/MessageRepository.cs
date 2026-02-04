using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Core;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class MessageRepository<T>(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : Repository<T>(dbContext, domainEventDispatcher) where T : Message<T> {

    protected override IQueryable<T> GetJoinedTable() =>
        this.GetJoinedTable().Include(x => x.SentBy);

}