using ReSR.Domain.Aggregates.Messages;
using ReSR.Application.Dtos;

namespace ReSR.Presentation.Api.Users.Mappers;

public static class PrivateMessageMapper
{
    public static PrivateMessageDto ToDto(this PrivateMessage msg) =>
        new(
            msg.Id,
            msg.SentBy.Id,
            msg.SentTo.Id,
            msg.Content,
            msg.CreatedAt
        );

    public static ConversationDto ToConversationDto(
    this IEnumerable<PrivateMessage> messages,
    Id currentUserId)
{
    if (!messages.Any())
        return new ConversationDto(
            "",
            Enumerable.Empty<PrivateMessageDto>()
        );

    var first = messages.First();

    var friend = first.SentBy.Id == currentUserId
        ? first.SentTo
        : first.SentBy;

    return new ConversationDto(
        friend.Username,
        messages.Select(m => m.ToDto())
    );
}
}