using System.Security.Cryptography.X509Certificates;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Messages;

namespace ReSR.Application.Services.Users.Definitions;

/// <summary>
/// A service for handling user private message.
/// </summary>
public interface IPrivateMessageService {

    /// <summary>
    /// Tries to send a private message.
    /// </summary>
    /// <returns>The sent private message.</returns>
    /// <param name="byUserId">The identifier of the user sending the message.</param>
    /// <param name="toUserId">The identifier of the user receiving the message.</param>
    /// <param name="content">The message's content.</param>
    public Task<IResponse<PrivateMessage>> TrySendAsync(Id byUserId, Id toUserId, string content);

    public Task<IResponse<IEnumerable<PrivateMessage>>> TryGetConversationMessages(Id userId, Id friendId);
}
