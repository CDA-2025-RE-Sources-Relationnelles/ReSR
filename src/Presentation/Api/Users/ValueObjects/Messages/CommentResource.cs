using ReSR.Domain.Aggregates.Messages;
using ReSR.Presentation.Api.Users.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Presentation.Api.Users.ValueObjects.Accounts;
using ReSR.Presentation.Api.Users.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Messages.ValueObjects;

namespace ReSR.Presentation.Api.Users.ValueObjects.Messages;
public class CommentResource(Comment from) : MessageResource<Comment>(from), IResource<CommentResource, Comment> {
    
    #region PROPERTIES

        public IEnumerable<string> Reports { get; } = from.Reports.Select(x => x.Content);

        public CommentLinks Links { get; } = new(
            Self              : GetLink(from),
            SentBy            : UserPrivateResource.GetLink(from.SentBy),
            CommentedResource : ResourceResource.GetLink(from.CommentedResource),
            Answers           : GetLink(from).WithSubRoute("answers"),
            AnsweredComment   : from.AnsweredComment is not null ? GetLink(from.AnsweredComment) : null,
            Report            : GetLink(from).WithSubRoute("report").WithMethod(Core.ValueObjects.HttpMethod.POST)
        );

        public readonly record struct CommentLinks(
            Link          Self,
            AnnotatedLink SentBy,
            AnnotatedLink CommentedResource,
            Link          Answers,
            Link?         AnsweredComment,
            Link          Report
        );

    #endregion
        #region METHODS

        public static implicit operator CommentResource(Comment from) => new (from);

        public static CommentResource From(Comment from) => from;
        public static IEnumerable<CommentResource> From(IEnumerable<Comment> from) => from.Select(From);

        public static Link GetLink(Comment from) => new (Core.ValueObjects.HttpMethod.GET, CommentController.ROUTE, from.Id);
        public static IEnumerable<Link> GetLinks(IEnumerable<Comment> from) => from.Select(GetLink);
        
    #endregion

}
