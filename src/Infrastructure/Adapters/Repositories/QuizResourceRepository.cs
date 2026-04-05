using Microsoft.EntityFrameworkCore;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Ports;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class QuizResourceRepository(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : ResourceRepository<QuizResource>(dbContext, domainEventDispatcher) {

    protected override IQueryable<QuizResource> GetJoinedTable() =>
        base.GetJoinedTable()
            .Include(x => x.Questions);
}