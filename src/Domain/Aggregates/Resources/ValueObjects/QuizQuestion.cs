namespace ReSR.Domain.Aggregates.Resources.ValueObjects;
public record QuizQuestion(
    int              Score,
    string           Content,
    List<QuizAnswer> Answers,
    int              Index = default
);