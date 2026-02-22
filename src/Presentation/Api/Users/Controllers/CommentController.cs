using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Users.Authorization;
using ReSR.Presentation.Api.Users.Extensions;
using ReSR.Presentation.Api.Users.ValueObjects.Messages;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
public class CommentController(
    ICommentService commentService,
    IRepository<Comment> repository
) : ControllerBase {

    public const string ROUTE = "/comments";
    #region DTOS

        public readonly record struct PostCommentAnswerDto(
            string Content
        );

        public readonly record struct ReportCommentAnswerDto(
            string Content
        );

    #endregion
    #region ROUTES
    
        [HttpGet(ROUTE + "/waiting-for-verification")]
        [EndpointSummary("Only accessible for authenticated users with comment verification permissions.")]
        [Authorize(Roles = nameof(UserPermissions.VerifyComments))]
        [EndpointDescription("Queries the reported comments.")]
        public Task<IResult> GetReportedCommentsAsync() =>
            commentService
                .GetAllReportedAsync()
                .ToResourceAsync<Comment, CommentResource>(Results.Ok);

        [HttpPost("{commentId}/answers")]
        [EndpointSummary("Only accessible for authenticated users.")]
        [Authorize(Roles = nameof(User), Policy = nameof(CommentReadAuthorizationRequirement))]
        [EndpointDescription("Posts an answer to the comment.")]
        public Task<IResult> PostCommentAnswerAsync(Id commentId, PostCommentAnswerDto dto) =>
            commentService
                .TryAnswerAsync(commentId, User.GetUserId()!.Value, dto.Content)
                .ToResourceAsync<Comment, CommentResource>(Results.Ok);

        [HttpPost("{commentId}/report")]
        [EndpointSummary("Only accessible for authenticated users.")]
        [Authorize(Roles = nameof(User), Policy = nameof(CommentReadAuthorizationRequirement))]
        [EndpointDescription("Reports the comment.")]
        public Task<IResult> ReportCommentAsync(Id commentId, ReportCommentAnswerDto dto) =>
            commentService
                .TryReportAsync(commentId, User.GetUserId()!.Value, dto.Content)
                .ToResourceAsync<Comment, CommentResource>(Results.Ok);

        [HttpPost("{commentId}/verify")]
        [EndpointSummary("Only accessible for authenticated users with comment verification permissions.")]
        [Authorize(Roles = nameof(UserPermissions.VerifyComments))]
        [EndpointDescription("Removes the comment's reports if any.")]
        public Task<IResult> VerifyCommentAsync(Id commentId) =>
            commentService
                .TryVerifyAsync(commentId)
                .ToResourceAsync<Comment, CommentResource>(Results.Ok);

        [HttpPost("{commentId}/reject")]
        [EndpointSummary("Only accessible for authenticated users with comment verification permissions.")]
        [Authorize(Roles = nameof(UserPermissions.VerifyComments))]
        [EndpointDescription("Removes the comment if it was reported.")]
        public Task<IResult> RejectCommentAsync(Id commentId) =>
            commentService
                .TryRejectAsync(commentId)
                .ToResultAsync(Results.Ok);

        [HttpGet("{commentId}")]
        [Authorize(Policy = nameof(CommentReadAuthorizationRequirement))]
        [EndpointDescription("Queries the comment.")]
        public Task<IResult> GetCommentAsync(Id commentId) =>
            repository.TryGetAsync(commentId).ToResourceAsync<Comment, CommentResource>(Results.Ok);

        [HttpGet("{commentId}/answers")]
        [Authorize(Policy = nameof(CommentReadAuthorizationRequirement))]
        [EndpointDescription("Queries the comment's answers.")]
        public Task<IResult> GetCommentAnswersAsync(Id commentId) =>
            repository.TryGetAsync(commentId).OnSuccessAsync(x => (IEnumerable<Comment>)x.Answers).ToResourceAsync<Comment, CommentResource>(Results.Ok);

    #endregion
    
}
