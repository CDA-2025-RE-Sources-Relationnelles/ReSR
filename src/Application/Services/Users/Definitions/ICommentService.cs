using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Messages;

namespace ReSR.Application.Services.Users.Definitions;

/// <summary>
/// A service for handling resource comments.
/// </summary>
public interface ICommentService {

    /// <summary>
    /// Tries to post a comment on a resource.
    /// </summary>
    /// <returns>The posted comment.</returns>
    /// <param name="byUserId">The identifier of the user sending the message.</param>
    /// <param name="content">The message's content.</param>
    /// <param name="toCommentId">The identifier of the answered comment, if any.</param>
    public Task<IResponse<Comment>> TryAnswerAsync(Id toCommentId, Id byUserId, string content);

    public Task<IEnumerable<Comment>> GetAllReportedAsync();

    /// <summary>
    /// Tries to reject a comment's verification.
    /// </summary>
    /// <returns>A successful response if the comment was rejected.</returns>
    /// <param name="id">The identifier of the verified comment.</param>
    public Task<IResponse> TryRejectAsync(Id id);

    /// <summary>
    /// Tries to confirm a comment's verification.
    /// </summary>
    /// <returns>A successful response if the comment was verified.</returns>
    /// <param name="id">The identifier of the verified comment.</param>
    public Task<IResponse<Comment>> TryVerifyAsync(Id id);

    public Task<IResponse<Comment>> TryReportAsync(Id id, Id userId, string content);

}
