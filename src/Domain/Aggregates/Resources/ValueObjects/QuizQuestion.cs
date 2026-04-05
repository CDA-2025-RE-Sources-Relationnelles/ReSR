namespace ReSR.Domain.Aggregates.Resources.ValueObjects;
public record QuizQuestion {
    public required int              Score   { get; init; }
    public required string           Content { get; init; }
    public required List<QuizAnswer> Answers { get; init; }
}