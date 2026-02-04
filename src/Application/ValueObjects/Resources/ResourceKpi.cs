namespace ReSR.Application.ValueObjects.Resources;
public readonly record struct ResourceKpi(
    uint Count,
    uint LikeCount,
    uint BookmarkCount,
    uint ExploitCount,
    uint CommentCount
);
