using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Managers.Definitions;
public interface IQuizResourceCommandService {

    public Task<IResponse<QuizResource>> TryCreateAsync(
        string                    title,
        Id                        categoryId,
        Relationships             relationships,
        string                    content,
        IEnumerable<QuizQuestion> questions
    );

    public Task<IResponse<QuizResource>> TryUpdateAsync(
        Id id,
        string?        title         = null,
        Id?            categoryId    = null,
        Relationships? relationships = null,
        string?        content       = null
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

    public Task<IResponse> TryDeleteAsync(Id id);
    public Task<IResponse<QuizResource>> TrySuspendAsync(Id id, bool value = true);

}
