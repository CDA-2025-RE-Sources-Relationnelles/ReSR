using ReSR.Domain.Aggregates.Messages;
using ReSR.Presentation.Api.Users.Controllers;
using ReSR.Presentation.Api.Core.ValueObjects;
using ReSR.Presentation.Api.Users.ValueObjects.Accounts;
using ReSR.Presentation.Api.Users.ValueObjects.Resources;

namespace ReSR.Presentation.Api.Users.ValueObjects.Messages;
public class PrivateMessageResource(PrivateMessage from) : MessageResource<PrivateMessage>(from), IResource<PrivateMessageResource, PrivateMessage> {
    
    #region PROPERTIES

        public PrivateMessageLinks Links { get; } = new(
            Self           : GetLink(from),
            SentBy         : UserResource.GetLink(from.SentBy),
            SentTo         : UserResource.GetLink(from.SentTo),
            QuotedResource : from.QuotedResource is not null ? ResourceResource.GetLink(from.QuotedResource) : null
        );

        public readonly record struct PrivateMessageLinks(
            Link           Self,
            AnnotatedLink  SentBy,
            AnnotatedLink  SentTo,
            AnnotatedLink? QuotedResource
        );

    #endregion
        #region METHODS

        public static implicit operator PrivateMessageResource(PrivateMessage from) => new (from);

        public static PrivateMessageResource From(PrivateMessage from) => from;
        public static IEnumerable<PrivateMessageResource> From(IEnumerable<PrivateMessage> from) => from.Select(From);

        public static Link GetLink(PrivateMessage from) => new (Core.ValueObjects.HttpMethod.GET, PrivateMessageController.ROUTE, from.Id);
        public static IEnumerable<Link> GetLinks(IEnumerable<PrivateMessage> from) => from.Select(GetLink);
        
    #endregion

}
