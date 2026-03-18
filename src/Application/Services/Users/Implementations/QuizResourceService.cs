using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Domain.Services.Implementations;

namespace ReSR.Application.Services.Users.Implementations;
internal class QuizResourceService(
    IResourceRepository<QuizResource> resourceRepository,
    IRepository<User> userRepository,
    IRepository<Comment> commentRepository,
    IRepository<Category> categoryRepository,
    IRepository<QuizSession> quizSessionRepository
) : ResourceService<QuizResource>(resourceRepository, userRepository, commentRepository),  IQuizResourceService{

    public Task<IResponse<QuizResource>> TryCreateAsync(
        Id                        ownerId,
        string                    title,
        string                    description,
        Id                        categoryId,
        Relationships             relationships,
        IEnumerable<QuizQuestion> questions,
        bool                      isPrivate
    ) => userRepository.TryGetAsync(ownerId).OnSuccessAsync(owner => categoryRepository
            .TryGetAsync(categoryId)
            .OnSuccessAsync(category => QuizResource.TryCreate(title, description, category, relationships, questions, isPrivate, owner))
        ).OnSuccessAsync(resourceRepository.TryAddAsync);

    public Task<IResponse<QuizResource>> TryUpdateAsync(
        Id id,
        string?                    title         = null,
        string?                    description   = null,
        Id?                        categoryId    = null,
        Relationships?             relationships = null,
        IEnumerable<QuizQuestion>? questions     = null
    ) => resourceRepository.TryUpdateAsync(id, async resource => {

        var response = Response.Success(resource);
        if (questions is not null) response = response.OnSuccess(x => x.WithQuestions(questions));
        if (categoryId is not null) response = await response.OnSuccessAsync(x =>
            categoryRepository
                .TryGetAsync(categoryId.Value)
                .OnSuccessAsync(category => x.WithCategory(category))
        );

        if (title is not null) response = response.OnSuccess(x => x.TryWithTitle(title));
        if (relationships is not null) response = response.OnSuccess(x => x.WithRelationships(relationships.Value));
        if (description is not null) response = response.OnSuccess(x => x.WithDescription(description));

        return response;

    });

    public Task<IResponse<QuizResource>> TryAddQuestionAsync(
        Id id,
        QuizQuestion question
    ) => resourceRepository.TryUpdateAsync(id, resource => resource.WithNewQuestion(question));
    
    public Task<IResponse<QuizResource>> TrySetQuestionAsync(
        Id id,
        int index,
        QuizQuestion question
    ) => resourceRepository.TryUpdateAsync(id, resource => resource.WithQuestion(index, question));

    public Task<IResponse<QuizResource>> TryRemoveQuestionAsync(
        Id id,
        int index
    ) => resourceRepository.TryUpdateAsync(id, resource => resource.WithoutQuestion(index));

    public Task<IResponse<QuizSession>> TryStartSessionAsync(Id id, Id userId, IEnumerable<Id> participantsId) =>
        resourceRepository.TryGetAsync(id).OnSuccessAsync(resource =>
            userRepository.TryGetAsync(userId).OnSuccessAsync(async user => {

                var participants = await userRepository.GetAllAsync(participantsId);

                return participants.All(x => user.Friends.Any(y => x.Id == y.Id))
                    ? participants.All(x => UserPermissionsService.TryVerifyUserResourceAccess(x, resource) is ISuccess)
                        ? await quizSessionRepository.TryAddAsync(QuizSession.Create(resource, [..participants, user]))
                        : Response.Failure<QuizSession>("Vous ne pouvez pas inviter des participants qui n'ont pas accès à la ressource !")
                    : Response.Failure<QuizSession>("Vous ne pouvez inviter que vos ami.e.s !");
            })
        );
}
