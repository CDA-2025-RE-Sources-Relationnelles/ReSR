using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using FluentResponse;
using FluentResponse.Interfaces;

namespace ReSR.Domain.Ports;

public interface IConversationRepository
{
    Task<Conversation?> GetByUsersAsync(Id user1Id, Id user2Id);
    Task<IResponse<Conversation>> AddAsync(Conversation conversation);
}