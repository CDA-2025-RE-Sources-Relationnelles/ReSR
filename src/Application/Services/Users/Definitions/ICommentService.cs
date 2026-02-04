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
    /// <param name="toResourceId">The identifier of the commented resource.</param>
    /// <param name="content">The message's content.</param>
    /// <param name="toCommentId">The identifier of the answered comment, if any.</param>
    public Task<IResponse<Comment>> TryPostAsync(Id byUserId, Id toResourceId, string content, Id? toCommentId = null);

    /// <summary>
    /// Tries to reject a comment's verification.
    /// </summary>
    /// <returns>A successful response if the comment was rejected.</returns>
    /// <param name="verifier">The identifier of the verifying user.</param>
    /// <param name="commentId">The identifier of the verified comment.</param>
    public Task<IResponse> TryRejectAsync(Id commentId, Id verifier);

    /// <summary>
    /// Tries to confirm a comment's verification.
    /// </summary>
    /// <returns>A successful response if the comment was verified.</returns>
    /// <param name="verifier">The identifier of the verifying user.</param>
    /// <param name="commentId">The identifier of the verified comment.</param>
    public Task<IResponse<Comment>> TryVerifyAsync(Id commentId, Id verifier);
}
