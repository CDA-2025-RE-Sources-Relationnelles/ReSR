using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Domain.Tests.Aggregates.Resources;
public class QuizResourceCreationTests {

    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed() {

        // Act
        var response = QuizResource.TryCreate(
            title         : "Quiz title",
            category      : Category.TryCreate("General").Unwrap(),
            relationships : Relationships.All,
            content       : "Some content",
            questions     : [QuizResourceTestFactory.ValidQuestion()]
        );

        // Assert
        Assert.IsType<ISuccess<QuizResource>>(response, exactMatch: false);

        var quiz = response.Unwrap();
        Assert.Equal("Quiz title", quiz.Title);
        Assert.Equal("Some content", quiz.Content);
        Assert.Single(quiz.Questions);
    }

    [Fact]
    public void TryCreate_WithNoQuestions_ShouldFail() {

        // Act
        var response = QuizResource.TryCreate(
            title         : "Quiz title",
            category      : Category.TryCreate("General").Unwrap(),
            relationships : Relationships.All,
            content       : "Some content",
            questions     : []
        );

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }

    [Fact]
    public void TryCreate_WithInvalidQuestion_ShouldFail() {

        var response = QuizResource.TryCreate(
            title         : "Quiz title",
            category      : Category.TryCreate("General").Unwrap(),
            relationships : Relationships.All,
            content       : "Some content",
            questions     : [new() {
                Score   = 20,
                Content = "Content",
                Answers = []
            }]
        );

        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
