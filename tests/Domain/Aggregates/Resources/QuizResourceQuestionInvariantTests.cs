using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Domain.Tests.Aggregates.Resources;
public class QuizQuestionInvariantTests {

    [Fact]
    public void Question_WithLessThanTwoAnswers_ShouldFail() {

        // Arrange
        var quiz = QuizResourceTestFactory.CreateValidQuiz();
        var badQuestion = new QuizQuestion {
            Content = "Invalid question",
            Score   = 20,
            Answers = [new() { IsCorrect = true, Content = "Content" }]
        };

        // Act
        var response = quiz.WithNewQuestion(badQuestion);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }

    [Fact]
    public void Question_WithNoCorrectAnswer_ShouldFail() {

        // Arrange
        var quiz = QuizResourceTestFactory.CreateValidQuiz();
        var badQuestion = new QuizQuestion {
            Content = "Invalid question",
            Score   = 20,
            Answers = [
                new() { IsCorrect = false, Content = "Content" },
                new() { IsCorrect = false, Content = "Content" },
                new() { IsCorrect = false, Content = "Content" },
            ]
        };

        // Act
        var response = quiz.WithNewQuestion(badQuestion);

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
