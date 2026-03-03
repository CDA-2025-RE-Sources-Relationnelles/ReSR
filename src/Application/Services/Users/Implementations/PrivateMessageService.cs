using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;
using ReSR.Application.Services.Users.Definitions;

namespace ReSR.Application.Services.Users.Implementations;

public class PrivateMessageService : IPrivateMessageService
{
    private readonly IConversationRepository conversationRepo;
    private readonly IPrivateMessageRepository messageRepo;
    private readonly IUserRepository userRepo;

    public PrivateMessageService(
        IConversationRepository conversationRepo,
        IPrivateMessageRepository messageRepo,
        IUserRepository userRepo)
    {
        this.conversationRepo = conversationRepo;
        this.messageRepo = messageRepo;
        this.userRepo = userRepo;
    }

    public async Task<IResponse<PrivateMessage>> TrySendAsync(Id senderId, Id receiverId, string content)
    {
        var senderResponse = await userRepo.GetByIdAsync(senderId);
        var receiverResponse = await userRepo.GetByIdAsync(receiverId);

        if (senderResponse is null || receiverResponse is null)
            return Response.Failure<PrivateMessage>("Utilisateur introuvable");

        var sender = senderResponse.Unwrap();
        var receiver = receiverResponse.Unwrap();

        var conversation = await conversationRepo.GetByUsersAsync(senderId, receiverId)
            ?? Conversation.TryCreate(sender, receiver).Unwrap();

        if (!conversation.Messages.Any())
            await conversationRepo.AddAsync(conversation);

        var message = PrivateMessage.TryCreate(sender, receiver, content, conversation).Unwrap();
        await messageRepo.AddAsync(message);

        return Response.Success(message);
    }

    public async Task<IResponse<IEnumerable<PrivateMessage>>> TryGetConversationMessages(Id userId, Id friendId)
    {
        var conversation = await conversationRepo.GetByUsersAsync(userId, friendId);
        if (conversation is null)
            return Response.Success(Enumerable.Empty<PrivateMessage>());

        var messages = conversation.Messages.OrderBy(m => m.CreatedAt);
        return Response.Success(messages);
    }
}