namespace ReSR.Application.Dtos;

public record PrivateMessageDto(
    Id Id,
    Id SenderId,
    Id ReceiverId,
    string Content,
    DateTime CreatedAt
);