using Microsoft.EntityFrameworkCore;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;
using System;
using System.Threading.Tasks;

namespace ReSR.Infrastructure.Adapters.Repositories;

internal class PrivateMessageRepository(
    DbContext dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : MessageRepository<PrivateMessage>(dbContext, domainEventDispatcher), IPrivateMessageRepository
{
    protected override IQueryable<PrivateMessage> GetJoinedTable() =>
        base.GetJoinedTable()
            .Include(x => x.SentTo)
            .Include(x => x.QuotedResource);

    public async Task<IResponse<PrivateMessage>> AddAsync(PrivateMessage message)
    {
        try
        {
            await dbContext.Set<PrivateMessage>().AddAsync(message);
            await dbContext.SaveChangesAsync();
            return Response.Success(message);
        }
        catch (Exception ex)
        {
            return Response.Failure<PrivateMessage>(ex);
        }
    }
}