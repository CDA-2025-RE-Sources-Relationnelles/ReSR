using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Ports;
using ReSR.Domain.Services.Implementations;

namespace ReSR.Application.Services.Users.Implementations;

internal class QuizSessionService(
    IRepository<QuizSession> quizSessionRepository,
    IRepository<User> userRepository
) : IQuizSessionService {

    public Task<IResponse<IEnumerable<QuizSession>>> TryGetAllAsync(
        Id userId
    ) => userRepository.TryGetAsync(userId).OnSuccessAsync(async user =>
        (await quizSessionRepository.GetAllAsync())
            .Where(x => x.Participations.Any(y => y.User.Id == userId))
            .Where(x => UserPermissionsService.TryVerifyUserResourceAccess(user, x.Resource) is ISuccess)
    );

    public Task<IResponse<QuizSession>> TryParticipateAsync(Id id, Id userId, int score) =>
        userRepository.TryGetAsync(userId).OnSuccessAsync(user =>
            quizSessionRepository.TryUpdateAsync(id, x => x.TryWithScore(user, score))
        );
}