using Microsoft.EntityFrameworkCore;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;

namespace ReSR.Infrastructure.Adapters.Repositories;

internal class PrivateMessageRepository(
    DbContext dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : MessageRepository<PrivateMessage>(dbContext, domainEventDispatcher), IPrivateMessageRepository
{
    protected override IQueryable<PrivateMessage> GetJoinedTable() =>
        base.GetJoinedTable()
            .Include(x => x.SentTo)
            .Include(x => x.SentBy)
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

    public async Task<IResponse<IEnumerable<PrivateMessage>>> GetMessagesBetweenUsersAsync(Id senderId, Id receiverId)
    {
        var messages = await dbContext.Set<PrivateMessage>()
            .Where(m =>
                (m.SentBy.Id == senderId && m.SentTo.Id == receiverId)
            || (m.SentBy.Id == receiverId && m.SentTo.Id == senderId)
            )
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        return Response.Success<IEnumerable<PrivateMessage>>(messages);
    }
}