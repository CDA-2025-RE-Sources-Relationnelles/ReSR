namespace ReSR.Domain.Aggregates.Resources.ValueObjects;
public record QuizAnswer {
    public required bool   IsCorrect { get; init; }
    public required string Content   { get; init; }
}