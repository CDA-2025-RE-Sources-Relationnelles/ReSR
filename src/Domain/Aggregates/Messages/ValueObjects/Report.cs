using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Domain.Aggregates.Messages.ValueObjects;
public record Report {
    public required         string   Content    { get; init; }
    public required virtual User     ReportedBy { get; init; }
    public                  DateTime ReportedAt { get; internal init; } = DateTime.UtcNow;
}