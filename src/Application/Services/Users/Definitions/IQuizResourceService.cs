using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Users.Definitions;
public interface IQuizResourceService : IResourceService<QuizResource> {

    public Task<IResponse<QuizResource>> TryCreateAsync(
        Id                        ownerId,
        string                    title,
        string                    description,
        Id                        categoryId,
        Relationships             relationships,
        IEnumerable<QuizQuestion> questions,
        bool                      isPrivate
    );

    public Task<IResponse<QuizResource>> TryUpdateAsync(
        Id id,
        string?                    title         = null,
        string?                    description   = null,
        Id?                        categoryId    = null,
        Relationships?             relationships = null,
        IEnumerable<QuizQuestion>? questions     = null
    );

    public Task<IResponse<QuizResource>> TryAddQuestionAsync(
        Id id,
        QuizQuestion question
    );
    
    public Task<IResponse<QuizResource>> TrySetQuestionAsync(
        Id id,
        int index,
        QuizQuestion question
    );

    public Task<IResponse<QuizResource>> TryRemoveQuestionAsync(
        Id id,
        int index
    );

    public Task<IResponse<QuizSession>> TryStartSessionAsync(
        Id id,
        Id userId,
        IEnumerable<Id> participantsId
    );
}
