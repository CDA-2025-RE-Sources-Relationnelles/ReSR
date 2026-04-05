using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Ports;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class QuizSessionRepository(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : Repository<QuizSession>(dbContext, domainEventDispatcher) {

    protected override IQueryable<QuizSession> GetJoinedTable() =>
        base.GetJoinedTable()
            .Include(x => x.Resource)
            .Include(x => x.Participations)
                .ThenInclude(x => x.User);

}