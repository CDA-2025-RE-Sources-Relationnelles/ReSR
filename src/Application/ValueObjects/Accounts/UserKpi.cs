using ReSR.Application.ValueObjects.Core;

namespace ReSR.Application.ValueObjects.Accounts;
public readonly record struct UserKpi(
    DateRange DateRange,
    long Count,
    long FriendshipCount,
    long BookmarkCount,
    long ResourceCount
);
