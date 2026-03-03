using Microsoft.EntityFrameworkCore;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;
using System.Threading.Tasks;

namespace ReSR.Infrastructure.Adapters.Repositories;

internal class ConversationRepository : IConversationRepository
{
    private readonly DbContext dbContext;
    private readonly DbSet<Conversation> set;

    public ConversationRepository(DbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
    {
        this.dbContext = dbContext;
        this.set = dbContext.Set<Conversation>();
    }

    public async Task<IResponse<Conversation>> AddAsync(Conversation conversation)
    {
        try
        {
            await set.AddAsync(conversation);
            await dbContext.SaveChangesAsync();
            return Response.Success(conversation);
        }
        catch (System.Exception ex)
        {
            return Response.Failure<Conversation>(ex);
        }
    }

    public async Task<Conversation?> GetByUsersAsync(Id user1Id, Id user2Id)
    {
        return await set
            .Include(c => c.Messages)
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstOrDefaultAsync(c =>
                (c.User1.Id == user1Id && c.User2.Id == user2Id) ||
                (c.User1.Id == user2Id && c.User2.Id == user1Id));
    }
}