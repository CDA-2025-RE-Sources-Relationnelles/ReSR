using ReSR.Domain.Aggregates.Messages;
using ReSR.Presentation.Api.Controllers;
using ReSR.Presentation.Api.Resources.Accounts;
using ReSR.Presentation.Api.Resources.Resources;

namespace ReSR.Presentation.Api.Resources.Messages;
public class CommentResource(Comment from) : MessageResource<Comment>(from), IResource<CommentResource, Comment> {
    
    #region PROPERTIES

        public IEnumerable<CommentResource> Answers { get; } = From(from.Answers);

        public CommentLinks Links { get; } = new(
            Self              : GetLink(from),
            SentBy            : UserResource.GetLink(from.SentBy),
            CommentedResource : ResourceResource.GetLink(from.CommentedResource),
            Answers           : GetLink(from).WithSubRoute("answers"),
            AnsweredComment   : from.AnsweredComment is not null ? GetLink(from.AnsweredComment) : null
        );

        public readonly record struct CommentLinks(
            ILink  Self,
            ILink  SentBy,
            ILink  CommentedResource,
            ILink  Answers,
            ILink? AnsweredComment
        );

    #endregion
        #region METHODS

        public static implicit operator CommentResource(Comment from) => new (from);

        public static CommentResource From(Comment from) => from;
        public static IEnumerable<CommentResource> From(IEnumerable<Comment> from) => from.Select(From);

        public static ILink GetLink(Comment from) => new Link(HttpMethod.GET, CommentController.ROUTE, from.Id);
        public static IEnumerable<ILink> GetLinks(IEnumerable<Comment> from) => from.Select(GetLink);
        
    #endregion

}
