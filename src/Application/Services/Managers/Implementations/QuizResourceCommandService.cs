using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.Logging;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
internal class QuizResourceCommandService(
    IRepository<QuizResource> repository,
    IRepository<Category> categoryRepository,
    ILogger<QuizResourceCommandService> logger
) : IQuizResourceCommandService {

    public Task<IResponse<QuizResource>> TryCreateAsync(
        string                    title,
        Id                        categoryId,
        Relationships             relationships,
        string                    content,
        IEnumerable<QuizQuestion> questions
    ) => categoryRepository
        .TryGetAsync(categoryId)
        .OnSuccessAsync(category => QuizResource.TryCreate(title, category, relationships, content, questions, isPrivate: false))
        .OnSuccessAsync(repository.TryAddAsync)
        .OnSuccessAsync(x => logger.LogInformation("Quiz resource created by manager: {@QuizResource} !", x));

    public Task<IResponse<QuizResource>> TryUpdateAsync(
        Id id,
        string?                    title         = null,
        Id?                        categoryId    = null,
        Relationships?             relationships = null,
        string?                    content       = null
    ) => repository.TryUpdateAsync(id, async resource => {

        var response = Response.Success(resource);

        if (title is not null) response = response.OnSuccess(x => x.TryWithTitle(title));
        if (categoryId is not null) response = await response.OnSuccessAsync(x =>
            categoryRepository
                .TryGetAsync(categoryId.Value)
                .OnSuccessAsync(category => x.WithCategory(category))
        );
        if (relationships is not null) response = response.OnSuccess(x => x.WithRelationships(relationships.Value));
        if (content is not null) response = response.OnSuccess(x => x.WithContent(content));

        return response;

    }).OnSuccessAsync(x => logger.LogInformation("Quiz resource updated by manager: {@QuizResource} !", x));

    public Task<IResponse<QuizResource>> TryAddQuestionAsync(
        Id id,
        QuizQuestion question
    ) => repository
        .TryUpdateAsync(id, resource => resource.WithNewQuestion(question))
        .OnSuccessAsync(x => logger.LogInformation("Quiz resource updated by manager: {@QuizResource} !", x));
    
    public Task<IResponse<QuizResource>> TrySetQuestionAsync(
        Id id,
        int index,
        QuizQuestion question
    ) => repository
        .TryUpdateAsync(id, resource => resource.WithQuestion(index, question))
        .OnSuccessAsync(x => logger.LogInformation("Quiz resource updated by manager: {@QuizResource} !", x));

    public Task<IResponse<QuizResource>> TryRemoveQuestionAsync(
        Id id,
        int index
    ) => repository
        .TryUpdateAsync(id, resource => resource.WithoutQuestion(index))
        .OnSuccessAsync(x => logger.LogInformation("Quiz resource updated by manager: {@QuizResource} !", x));

    public Task<IResponse> TryDeleteAsync(Id id) =>
        repository
            .TryDeleteAsync(id)
            .OnSuccessAsync(() => logger.LogInformation("Quiz resource with id {@Id} deleted by manager !", id));

    public Task<IResponse<QuizResource>> TrySuspendAsync(uint id, bool value = true) =>
        repository
            .TryUpdateAsync(id, x => x.WithSuspension(value))
            .OnSuccessAsync(x => logger.LogInformation("Quiz resource suspsension changed by manager: {@QuizResource} !", x));
}
