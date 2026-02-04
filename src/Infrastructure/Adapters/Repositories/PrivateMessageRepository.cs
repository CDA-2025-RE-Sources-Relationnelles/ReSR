using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Core;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class PrivateMessageRepository(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : MessageRepository<PrivateMessage>(dbContext, domainEventDispatcher) {

    protected override IQueryable<PrivateMessage> GetJoinedTable() =>
        base.GetJoinedTable()
            .Include(x => x.SentTo)
            .Include(x => x.QuotedResource);

}