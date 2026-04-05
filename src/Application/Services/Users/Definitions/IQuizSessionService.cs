using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.QuizSessions;

namespace ReSR.Application.Services.Users.Definitions;

/// <summary>
/// A service for handling resource comments.
/// </summary>
public interface IQuizSessionService {

    public Task<IResponse<IEnumerable<QuizSession>>> TryGetAllAsync(Id userId);

    /// <summary>
    /// Tries to participate to a quiz session.
    /// </summary>
    /// <returns>The updated session details.</returns>
    /// <param name="score">The score of the participation.</param>
    /// <param name="userId">The identifier of the user participating to the session.</param>
    /// <param name="quizSessionId">The identifier of the quiz session.</param>
    public Task<IResponse<QuizSession>> TryParticipateAsync(Id quizSessionId, Id userId, int score);

}
