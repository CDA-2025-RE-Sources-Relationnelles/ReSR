using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Aggregates.Categories;
using FluentResponse;
using FluentResponse.Interfaces;

internal static class QuizResourceTestFactory {
    public static QuizQuestion ValidQuestion() =>
        new() {
            Content = "Question",
            Score   = 20,
            Answers = [
                new() { IsCorrect = true,  Content = "Foo" },
                new() { IsCorrect = false, Content = "Bar" },
                new() { IsCorrect = false, Content = "Baz" },
            ]
        };

    public static QuizResource CreateValidQuiz(IEnumerable<QuizQuestion>? questions = null) {
        var response = QuizResource.TryCreate(
            title         : "Valid quiz",
            category      : Category.TryCreate("Valid category").Unwrap(),
            relationships : Relationships.All,
            content       : "Quiz content",
            questions     : questions ?? [ValidQuestion()],
            isPrivate     : true
        );

        Assert.IsType<ISuccess<QuizResource>>(response, exactMatch: false);
        return response.Unwrap();
    }
}
