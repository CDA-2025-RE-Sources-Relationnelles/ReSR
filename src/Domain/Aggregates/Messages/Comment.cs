using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Messages;

/// <summary>
/// A resource comment.
/// </summary>
public record Comment : Message<Comment> {

    #region PROPERTIES

        /// <summary> The commented resource. </summary>
        public virtual Resource CommentedResource { get; internal init; } = null!;

        /// <summary> The comment answered by the message, if any. </summary>
        public virtual Comment? AnsweredComment { get; internal init; }

        /// <summary> The comment's answers. </summary>
        public virtual ICollection<Comment> Answers { get; internal init; } = [];

        /// <summary> The reports registered for this comment. </summary>
        public ICollection<Report> Reports { get; internal init; } = [];

    #endregion
    #region CONSTRUCTORS
            
        public static IResponse<Comment> TryCreate(
            User     sentBy,
            string   content,
            Resource commentedResource,
            Comment? answeredComment = null
        ) => TryVerifyContentInvariant(content)
                .OnSuccess(() => new Comment {
                    SentBy            = sentBy,
                    Content           = content,
                    CommentedResource = commentedResource,
                    AnsweredComment   = answeredComment
                });

    #endregion
    #region METHODS

        /// <returns> A copy of the comment with the given report if it wasn't already reported by the user. </returns>
        public virtual IResponse<Comment> TryWithReport(User by, string content) =>
            !this.Reports.Any(x => x.ReportedBy.Id == by.Id)
                ? Response.Success(this with { Reports = [.. this.Reports, new Report { ReportedBy = by, Content = content }] })
                : Response.Failure<Comment>(new InvariantException("Un commentaire ne peut être signalé qu'une fois par utilisateur !"));

        /// <returns> A copy of the comment without reports from the given user. </returns>
        public virtual Comment WithoutReport(User by) =>
            this with { Reports = [.. this.Reports.Where(x => x.ReportedBy.Id != by.Id)] };

    #endregion

}