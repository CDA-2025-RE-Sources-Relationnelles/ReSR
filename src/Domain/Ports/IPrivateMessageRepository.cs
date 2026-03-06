using ReSR.Domain.Aggregates.Messages;
using FluentResponse;
using FluentResponse.Interfaces;

namespace ReSR.Domain.Ports;

public interface IPrivateMessageRepository : IRepository<PrivateMessage>
{
    Task<IResponse<PrivateMessage>> AddAsync(PrivateMessage message);
    Task<IResponse<IEnumerable<PrivateMessage>>> GetMessagesBetweenUsersAsync(Id senderId, Id receiverId);
}