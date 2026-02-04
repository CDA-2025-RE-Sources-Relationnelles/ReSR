using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Domain.Aggregates.QuizSessions.ValueObjects;
public record QuizParticipation {
    public                  int? Score { get; init; }
    public required virtual User User  { get; init; }
};