using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Users.Implementations;
internal class CommentService(
    IRepository<Comment> commentRepository,
    IRepository<User> userRepository
) : ICommentService {
    public Task<IResponse<Comment>> TryAnswerAsync(Id toCommentId, Id byUserId, string content) =>
        commentRepository.TryGetAsync(toCommentId).OnSuccessAsync(answeredComment =>
            userRepository.TryGetAsync(byUserId).OnSuccessAsync(sentBy =>
                Comment.TryCreate(sentBy, content, answeredComment.CommentedResource, answeredComment)
            )
        ).OnSuccessAsync(commentRepository.TryAddAsync);

    public async Task<IEnumerable<Comment>> GetAllReportedAsync() =>
        (await commentRepository.GetAllAsync()).Where(x => x.Reports.Count > 0);

    public Task<IResponse> TryRejectAsync(Id id) =>
        commentRepository.TryGetAsync(id).OnSuccessAsync(async comment =>
            comment.Reports.Count is > 0
                ? await commentRepository.TryDeleteAsync(id)
                : Response.Failure("Le commentaire n'est pas signalé !")
        );

    public Task<IResponse<Comment>> TryVerifyAsync(Id id) =>
        commentRepository.TryUpdateAsync(id, x => x.WithoutReports());

    public Task<IResponse<Comment>> TryReportAsync(Id id, Id userId, string content) =>
        userRepository.TryGetAsync(userId).OnSuccessAsync(user =>
            commentRepository.TryUpdateAsync(id, x => x.TryWithReport(user, content))
        );

    public Task<IEnumerable<Comment>> GetAnswersAsync(Id commentId) =>
        commentRepository.GetAllAsync(x => x.AnsweredComment != null && x.AnsweredComment.Id == commentId);
}
