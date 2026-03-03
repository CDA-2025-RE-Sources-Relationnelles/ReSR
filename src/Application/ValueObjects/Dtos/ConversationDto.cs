namespace ReSR.Application.Dtos;

public record ConversationDto(
    string FriendUsername,
    IEnumerable<PrivateMessageDto> Messages
);