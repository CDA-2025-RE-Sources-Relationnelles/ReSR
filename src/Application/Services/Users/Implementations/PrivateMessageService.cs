using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;
using ReSR.Application.Services.Users.Definitions;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks.Dataflow;

namespace ReSR.Application.Services.Users.Implementations;

internal class PrivateMessageService(
    IPrivateMessageRepository messageRepo,
    IRepository<User> userRepo
) : IPrivateMessageService
{
    public Task<IResponse<PrivateMessage>> TrySendAsync(
        Id senderId, 
        Id receiverId, 
        string content
    ){
            return userRepo.TryGetAsync(senderId)
                .OnSuccessAsync(sender => 
                    userRepo.TryGetAsync(receiverId)
                        .OnSuccessAsync(receiver =>
                            PrivateMessage
                                .TryCreate(sender, receiver, content)
                                .OnSuccessAsync(messageRepo.TryAddAsync)
                        )
                );
        }

    public Task<IResponse<IEnumerable<PrivateMessage>>> TryGetMessagesBetweenAsync(
        Id senderId, 
        Id receiverId
    ){
        return messageRepo.GetMessagesBetweenUsersAsync(senderId, receiverId);
    }
}