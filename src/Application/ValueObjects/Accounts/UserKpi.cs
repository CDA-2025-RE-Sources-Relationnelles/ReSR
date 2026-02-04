namespace ReSR.Application.ValueObjects.Accounts;
public readonly record struct UserKpi(
    uint Count,
    uint FriendCount,
    uint LikeCount,
    uint BookmarkCount,
    uint ResourceCount
);
