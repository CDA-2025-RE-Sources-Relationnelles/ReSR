using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Accounts.Events;
public record UserMutuallyLiked(
    Id   UserId,
    Id   ByUserId
) : DomainEvent;