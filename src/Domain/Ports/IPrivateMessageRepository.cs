using ReSR.Domain.Aggregates.Messages;
using FluentResponse;
using FluentResponse.Interfaces;

namespace ReSR.Domain.Ports;

public interface IPrivateMessageRepository
{
    Task<IResponse<PrivateMessage>> AddAsync(PrivateMessage message);
}