namespace ReSR.Presentation.Api.Users.Dtos;

public record SendMessageDto(
    uint ReceiverId,
    string Content
);