using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Resources;
public record TextResource : Resource, IAggregateRoot<TextResource> {

    #region PROPERTIES

        /// <summary> The resource's text content. </summary>
        public string Content { get; internal init; } = null!;

    #endregion
    #region CONSTRUCTORS

        public static IResponse<TextResource> TryCreate(
            string              title,
            Category            category,
            IEnumerable<string> tags,
            string              content,
            bool                isPrivate = true,
            User?               owner     = null
        ) => TryVerifyTitleInvariant(title)
                .OnSuccess(() => TryVerifyTagsInvariant(tags))
                .OnSuccess(() => new TextResource {
                    Title      = title,
                    Category   = category,
                    RawTags    = string.Join(';', tags.ToHashSet()),
                    Content    = content,
                    Owner      = owner,
                    Visibility = isPrivate
                        ? Visibility.Private 
                        : Visibility.WaitingForVerification
                });

    #endregion
    #region METHODS
            
        /// <returns> A copy of the resource with the given content. </returns>
        public virtual TextResource WithContent(string value) =>
            this with { Content = value };

        #region OVERRIDES

            public new IResponse<TextResource> TryWithTitle(string value) =>
                (IResponse<TextResource>)base.TryWithTitle(value);

            public new IResponse<TextResource> TryWithTag(string value) =>
                (IResponse<TextResource>)base.TryWithTag(value);

            public new TextResource WithoutTag(string value) =>
                (TextResource)base.WithoutTag(value);

            public new TextResource WithRelationships(Relationships value) =>
                (TextResource)base.WithRelationships(value);

            public new TextResource WithLikeFrom(User from, bool value) =>
                (TextResource)base.WithLikeFrom(from, value);

            public new TextResource WithBookmarkFrom(User from, bool value) =>
                (TextResource)base.WithBookmarkFrom(from, value);

            public new TextResource WithExploitFrom(User from, bool value) =>
                (TextResource)base.WithExploitFrom(from, value);

            public new TextResource AsPrivate() =>
                (TextResource)base.AsPrivate();

            public new TextResource AsPublic() =>
                (TextResource)base.AsPublic();

            public new TextResource WithSuspension(bool value = true) =>
                (TextResource)base.WithSuspension(value);

            public new IResponse<TextResource> TryWithNewVerifyingUser(User value) =>
                (IResponse<TextResource>)base.TryWithNewVerifyingUser(value);

            public new IResponse<TextResource> TryWithConfirmedVerification() =>
                (IResponse<TextResource>)base.TryWithConfirmedVerification();

            public new IResponse<TextResource> TryWithRejectedVerification() =>
                (IResponse<TextResource>)base.TryWithRejectedVerification();

            public new IResponse<TextResource> TryWithCanceledVerification() =>
                (IResponse<TextResource>)base.TryWithCanceledVerification();

            public new TextResource WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) =>
                (TextResource)base.WithConsumedEvents(out domainEvents);

        #endregion
    #endregion

}
