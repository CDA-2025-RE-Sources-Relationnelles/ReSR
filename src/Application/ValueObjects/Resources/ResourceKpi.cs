using ReSR.Application.ValueObjects.Core;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.ValueObjects.Resources;
public readonly record struct ResourceKpi(
    DateRange DateRange,
    Relationships Relationship,
    string? CategoryName,
    long Count,
    long LikeCount,
    long BookmarkCount,
    long ExploitCount,
    long CommentCount
);
