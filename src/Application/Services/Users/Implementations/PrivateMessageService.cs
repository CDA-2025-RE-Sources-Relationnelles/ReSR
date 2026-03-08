using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Resources;

namespace ReSR.Application.Services.Users.Implementations;

internal class PrivateMessageService(
    IPrivateMessageRepository messageRepo,
    IRepository<User> userRepo,
    IRepository<Resource> resourceRepo
) : IPrivateMessageService
{
    public Task<IResponse<PrivateMessage>> TrySendAsync(
    Id senderId, 
    Id receiverId, 
    string content,
    Id? resourceId = null
    ){
        return userRepo.TryGetAsync(senderId)
            .OnSuccessAsync(sender => 
                userRepo.TryGetAsync(receiverId)
                    .OnSuccessAsync(receiver =>
                    {
                        if (!sender.Friends.Any(f => f.Id == receiver.Id))
                            return Task.FromResult(
                                Response.Failure<PrivateMessage>("Vous devez être amis pour vous envoyer des messages !")
                            );

                        if (resourceId is null)
                            return PrivateMessage
                                .TryCreate(sender, receiver, content, null)
                                .OnSuccessAsync(messageRepo.TryAddAsync);
                        

                        return resourceRepo.TryGetAsync(resourceId.Value)
                            .OnSuccessAsync(resource =>
                                PrivateMessage
                                    .TryCreate(sender, receiver, content, resource)
                                    .OnSuccessAsync(messageRepo.TryAddAsync)
                            );
                        }
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