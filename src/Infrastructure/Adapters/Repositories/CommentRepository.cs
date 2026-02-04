using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Core;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class CommentRepository(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : MessageRepository<Comment>(dbContext, domainEventDispatcher) {

    protected override IQueryable<Comment> GetJoinedTable() =>
        base.GetJoinedTable()
            .Include(x => x.CommentedResource)
            .Include(x => x.AnsweredComment)
            .Include(x => x.Reports)
                .ThenInclude(x => x.ReportedBy);

}