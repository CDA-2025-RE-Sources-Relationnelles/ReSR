using System.Data.Common;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace ReSR.Application.Services.Users.Implementations;

internal class PrivateMessageService(
    IRepository<PrivateMessage> messageRepository,
    IRepository<User> userRepository
) : IPrivateMessageService
{
    public Task<IResponse<PrivateMessage>> TrySendAsync(Id byUserId, Id toUserId, string contect) => 
        userRepository.TryGetAsync(byUserId).OnSuccessAsync(sender => 
            userRepository.TryGetAsync(toUserId).OnSuccessAsync(receiver => 
                PrivateMessage.TryCreate(sender, receiver, content)
            )
        ).OnSuccessAsync(messageRepository.TryAddAsync);

    public async Task<IResponse<IEnumerable<PrivateMessage>>> TryGetAll(Id userId)
    {
        var messages = await messageRepository.GetAllAsync(X => X.SentBy.Id == userId || X.SendTo.Id == userId);
        return WebResponse.Success(messages);
    }
}