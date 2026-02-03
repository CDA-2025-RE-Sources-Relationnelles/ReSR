using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Domain.Aggregates.QuizSessions.ValueObjects;
public record QuizParticipation(
    User User,
    int? Score = null
);