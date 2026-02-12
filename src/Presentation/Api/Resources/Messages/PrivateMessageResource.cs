using ReSR.Domain.Aggregates.Messages;
using ReSR.Presentation.Api.Resources.Accounts;
using ReSR.Presentation.Api.Resources.Resources;

namespace ReSR.Presentation.Api.Resources.Messages;
public class PrivateMessageResource(PrivateMessage from) : MessageResource<PrivateMessage>(from), IResource<PrivateMessageResource, PrivateMessage> {
    
    #region PROPERTIES

        public PrivateMessageLinks Links { get; } = new(
            Self           : GetLink(from),
            SentBy         : UserResource.GetLink(from.SentBy),
            SentTo         : UserResource.GetLink(from.SentTo),
            QuotedResource : from.QuotedResource is not null ? ResourceResource.GetLink(from.QuotedResource) : null
        );

        public readonly record struct PrivateMessageLinks(
            ILink  Self,
            ILink  SentBy,
            ILink  SentTo,
            ILink? QuotedResource
        );

    #endregion
        #region METHODS

        public static implicit operator PrivateMessageResource(PrivateMessage from) => new (from);

        public static PrivateMessageResource From(PrivateMessage from) => from;
        public static IEnumerable<PrivateMessageResource> From(IEnumerable<PrivateMessage> from) => from.Select(From);

        public static ILink GetLink(PrivateMessage from) => new Link(HttpMethod.GET, PrivateMessageController.ROUTE, from.Id);
        public static IEnumerable<ILink> GetLinks(IEnumerable<PrivateMessage> from) => from.Select(GetLink);
        
    #endregion

}
