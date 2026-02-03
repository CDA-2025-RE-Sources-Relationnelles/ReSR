namespace ReSR.Domain.Aggregates.Resources.ValueObjects;
public record QuizAnswer(
    bool   IsCorrect,
    string Content,
    int    Index = default
);