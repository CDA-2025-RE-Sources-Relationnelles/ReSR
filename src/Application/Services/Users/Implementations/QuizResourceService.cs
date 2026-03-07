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
        Id                        categoryId,
        Relationships             relationships,
        string                    content,
        IEnumerable<QuizQuestion> questions,
        bool                      isPrivate
    ) => userRepository.TryGetAsync(ownerId).OnSuccessAsync(owner => categoryRepository
            .TryGetAsync(categoryId)
            .OnSuccessAsync(category => QuizResource.TryCreate(title, category, relationships, content, questions, isPrivate, owner))
        ).OnSuccessAsync(resourceRepository.TryAddAsync);

    public Task<IResponse<QuizResource>> TryUpdateAsync(
        Id id,
        string?                    title         = null,
        Id?                        categoryId    = null,
        Relationships?             relationships = null,
        string?                    content       = null
    ) => resourceRepository.TryUpdateAsync(id, async resource => {

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
                    : Response.Failure<QuizSession>("Vous ne pouvez invitez que vos ami.e.s !");
            })
        );
}
