using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources;

namespace ReSR.Domain.Tests.Aggregates.Resources;
public class QuizResourceCreationTests {

    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed() {

        // Act
        var response = QuizResource.TryCreate(
            title     : "Quiz title",
            category  : Category.TryCreate("General").Unwrap(),
            tags      : ["tag1", "tag2"],
            content   : "Some content",
            questions : [QuizResourceTestFactory.ValidQuestion()]
        );

        // Assert
        Assert.IsType<ISuccess<QuizResource>>(response, exactMatch: false);

        var quiz = response.Unwrap();
        Assert.Equal("Quiz title", quiz.Title);
        Assert.Equal("Some content", quiz.Content);
        Assert.Equal("tag1;tag2", quiz.RawTags);
        Assert.Single(quiz.Questions);
        Assert.Equal(1, quiz.Questions[0].Index);
        Assert.Equal(1, quiz.Questions[0].Answers[0].Index);
    }

    [Fact]
    public void TryCreate_WithNoQuestions_ShouldFail() {

        // Act
        var response = QuizResource.TryCreate(
            title     : "Quiz title",
            category  : Category.TryCreate("General").Unwrap(),
            tags      : ["tag1", "tag2"],
            content   : "Some content",
            questions : []
        );

        // Assert
        Assert.IsType<IFailure>(response, exactMatch: false);
    }

    [Fact]
    public void TryCreate_WithInvalidQuestion_ShouldFail() {

        var response = QuizResource.TryCreate(
            title     : "Quiz title",
            category  : Category.TryCreate("General").Unwrap(),
            tags      : ["tag1", "tag2"],
            content   : "Some content",
            questions : [new (
                Score   : 20,
                Content : "Content",
                Answers : []
            )]
        );

        Assert.IsType<IFailure>(response, exactMatch: false);
    }
}
