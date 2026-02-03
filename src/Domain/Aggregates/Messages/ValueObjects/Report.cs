using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Domain.Aggregates.Messages.ValueObjects;
public record Report(
    User   ReportedBy,
    string Content
) { public DateTime ReportedAt { get; init; }}