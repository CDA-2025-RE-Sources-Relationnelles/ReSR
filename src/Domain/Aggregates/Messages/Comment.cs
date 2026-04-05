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
        public virtual ICollection<Comment> Answers { get; internal set; } = [];

        /// <summary> The reports registered for this comment. </summary>
        public ICollection<Report> Reports { get; internal set; } = [];

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
        public virtual IResponse<Comment> TryWithReport(User by, string content) {
            if (!this.Reports.Any(x => x.ReportedBy.Id == by.Id)) {

                List<Report> reports = [.. this.Reports, new Report { ReportedBy = by, Content = content }];
                this.Reports = reports;
                return Response.Success(this);

            } else return Response.Failure<Comment>(new InvariantException("Un commentaire ne peut être signalé qu'une fois par utilisateur !"));
        }

        public virtual Comment WithoutReports() {
            List<Report> reports = [.. this.Reports];
            reports.Clear();
            this.Reports = reports;
            return this;
        }

        /// <returns> A copy of the comment without reports from the given user. </returns>
        public virtual Comment WithoutReport(User by) =>
            this with { Reports = [.. this.Reports.Where(x => x.ReportedBy.Id != by.Id)] };

    #endregion

}