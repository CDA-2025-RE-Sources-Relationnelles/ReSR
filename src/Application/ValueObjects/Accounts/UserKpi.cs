using ReSR.Application.ValueObjects.Core;

namespace ReSR.Application.ValueObjects.Accounts;
public readonly record struct UserKpi(
    DateRange DateRange,
    int Count,
    int FriendshipCount,
    int BookmarkCount,
    int ResourceCount
);
