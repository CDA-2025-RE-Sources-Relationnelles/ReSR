using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.Resources.Events;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;
using ReSR.Domain.Extensions;

namespace ReSR.Domain.Aggregates.Resources;

/// <summary>
/// A resource base record.
/// </summary>
public abstract record Resource(Id Id = default) : IAggregateRoot<Resource> {

    #region PROPERTIES

        /// <summary> The resource's title </summary>
        public string Title { get; internal init; } = null!;

        /// <summary> The resource's category </summary>
        public Category Category { get; internal init; } = null!;

        /// <summary> The resource's targeted relationships. </summary>
        public Relationships Relationships { get; internal init; }

        /// <summary> The resource's visibility. </summary>
        public Visibility Visibility { get; internal init; }
        


        /// <summary> The resource's tags separated by a ';'. </summary>
        internal string RawTags { get; init; } = string.Empty;

        /// <summary> The resource's tags. </summary>
        public virtual IEnumerable<string> Tags => this.RawTags.Split(';', options: StringSplitOptions.RemoveEmptyEntries);


        /// <summary> The instant at which the resource was published. </summary>
        public DateTime PublishedAt { get; internal init; } = DateTime.UtcNow;

        /// <summary> The instant at which the resource was last edited. </summary>
        public DateTime EditedAt { get; internal init; } = DateTime.UtcNow;



        /// <summary> The resource's owner, if any. </summary>
        public virtual User? Owner { get; internal init; }

        /// <summary> The users that liked this resource. </summary>
        public virtual ICollection<User> LikedBy { get; internal init; } = new HashSet<User>();

        /// <summary> The users that bookmarked this resource. </summary>
        public virtual ICollection<User> BookmarkedBy { get; internal init; } = new HashSet<User>();

        /// <summary> The users that exploited this resource. </summary>
        public virtual ICollection<User> ExploitedBy { get; internal init; } = new HashSet<User>();

        /// <summary> The users that is verifying this resource before being publicly available. </summary>
        public virtual User? VerifyingUser { get; internal init; }

        /// <summary> The resource's comments. </summary>
        public virtual ICollection<Comment> Comments { get; internal init; } = [];



        /// <summary> The amount of likes on this resource. </summary>
        public virtual uint LikeCount => (uint)this.LikedBy.Count;

        /// <summary> The amount of bookmarks on this resource. </summary>
        public virtual uint BookmarkCount => (uint)this.BookmarkedBy.Count;

        /// <summary> The amount of exploits on this resource. </summary>
        public virtual uint ExploitCount => (uint)this.ExploitedBy.Count;



    #endregion
    #region METHODS
        #region UPDATES

            /// <returns> A copy of the resource with a new title if valid. </returns>
            public virtual IResponse<Resource> TryWithTitle(string value) =>
                TryVerifyTitleInvariant(value).OnSuccess(() => this with {
                    EditedAt = DateTime.UtcNow,
                    Title    = value,
                });

            /// <returns> A copy of the resource with an additionnal tag if valid. </returns>
            public virtual IResponse<Resource> TryWithTag(string value) =>
                TryVerifyTagInvariant(value).OnSuccess(() => this with {
                    EditedAt = DateTime.UtcNow,
                    RawTags  = this.Tags.Contains(value)
                        ? RawTags
                        : string.Join(';', [..this.Tags, value])
                });

            /// <returns> A copy of the resource without a given tag. </returns>
            public virtual Resource WithoutTag(string value) =>
                this with {
                    EditedAt = DateTime.UtcNow,
                    RawTags  = this.Tags.Contains(value)
                        ? string.Join(';', this.Tags.Where(x => x != value))
                        : RawTags
                };

            /// <returns> A copy of the resource with the given relationships. </returns>
            public virtual Resource WithRelationships(Relationships value) =>
                this with {
                    EditedAt      = DateTime.UtcNow,
                    Relationships = value
                };

            /// <returns> A successful response if the relationships matches the resource's. </returns>
            public virtual IResponse TryVerifyRelationships(Relationships value) =>
                this.Relationships.HasFlag(value)
                ? Response.Success()
                : Response.Failure($"La ressource ne correspond pas aux relations suivantes '{value.GetUniqueValues()}'");



            /// <returns> A copy of the resource with a like set or unset from the given user. </returns>
            public virtual Resource WithLikeFrom(User from, bool value) =>
                value
                ? this with { LikedBy = [.. this.LikedBy, from], }
                : this with { LikedBy = [.. this.LikedBy.Where(x => x.Id != from.Id)] };

            /// <returns> A copy of the resource with a bookmark set or unset from the given user. </returns>
            public virtual Resource WithBookmarkFrom(User from, bool value) =>
                value
                ? this with { BookmarkedBy = [..this.BookmarkedBy, from], }
                : this with { BookmarkedBy = [.. this.BookmarkedBy.Where(x => x.Id != from.Id)] };

            /// <returns>
            /// A copy of the resource with an exploit set or unset from the given user.
            /// If the user had bookmarked the resource, it will be removed from their bookmarked resource list.
            /// </returns>
            public virtual Resource WithExploitFrom(User from, bool value) =>
                value
                ? this with { ExploitedBy = [.. this.ExploitedBy, from], BookmarkedBy = [.. this.BookmarkedBy.Where(x => x.Id != from.Id)], }
                : this with { ExploitedBy = [.. this.ExploitedBy.Where(x => x.Id != from.Id)] };



            public virtual bool IsLikedBy(User by) => this.LikedBy.Any(x => x.Id == by.Id);
            public virtual bool IsExploitedBy(User by) => this.ExploitedBy.Any(x => x.Id == by.Id);
            public virtual bool IsBookmarkedBy(User by) => this.BookmarkedBy.Any(x => x.Id == by.Id);


            /// <returns> A copy of the resource with a private visibility set. </returns>
            public virtual Resource AsPrivate() =>
                this with { Visibility = Visibility.Private };

            /// <returns> A copy of the resource with a public visibility set. </returns>
            public virtual Resource AsPublic() =>
                this with { Visibility = Visibility.Public };
                
            /// <returns> A copy of the resource with suspension set or unset. </returns>
            public virtual Resource WithSuspension(bool value = true) =>
                this with { Visibility = value ? Visibility | Visibility.Suspended : Visibility & ~Visibility.Suspended };



            /// <returns> A copy of the resource with the given user as verifier if none. </returns>
            public virtual IResponse<Resource> TryWithNewVerifyingUser(User value) =>
                this.VerifyingUser is null
                ? Response.Success(this with { VerifyingUser = value })
                : Response.Failure<Resource>(new InvariantException($"La ressource est déjà en train d'être vérifiée par {this.VerifyingUser.Email} !"));

            /// <returns> A copy of the resource with confirmed verification if being verified. </returns>
            public virtual IResponse<Resource> TryWithConfirmedVerification() =>
                this.VerifyingUser is not null
                ? Response.Success(this with {
                    VerifyingUser = null,
                    Visibility    = Visibility.Public,
                    DomainEvents  = [..this.DomainEvents, new ResourceVerified(this.Id)]
                }) : Response.Failure<Resource>(new InvariantException($"La ressource n'est vérifiée par aucun utilisateur !"));

            /// <returns> A copy of the resource with rejected verification if being verified. </returns>
            public virtual IResponse<Resource> TryWithRejectedVerification() =>
                this.VerifyingUser is not null
                ? Response.Success(this with {
                    VerifyingUser = null,
                    Visibility    = Visibility.Suspended,
                    DomainEvents  = [..this.DomainEvents, new ResourceRejected(this.Id)]
                }) : Response.Failure<Resource>(new InvariantException($"La ressource n'est vérifiée par aucun utilisateur !"));

            /// <returns> A copy of the resource with canceled verification if being verified. </returns>
            public virtual IResponse<Resource> TryWithCanceledVerification() =>
                this.VerifyingUser is not null
                ? Response.Success(this with { VerifyingUser = null })
                : Response.Failure<Resource>(new InvariantException($"La ressource n'est vérifiée par aucun utilisateur !"));

        #endregion
        #region INVARIANTS

            protected static IResponse TryVerifyTitleInvariant(string value) =>
                value.Trim().Length >= 4
                ? Response.Success()
                : Response.Failure(new InvariantException("Un titre de ressource doit contenir au moins 4 caractères !"));

            protected static IResponse TryVerifyTagsInvariant(IEnumerable<string> values) {
                foreach (var tag in values) {
                    var response = TryVerifyTagInvariant(tag);
                    if (response is IFailure) return response;
                }

                return Response.Success();
            }

            protected static IResponse TryVerifyTagInvariant(string value) =>
                value.All(x => char.IsAsciiLetterOrDigit(x) || x == '-') && !string.IsNullOrWhiteSpace(value)
                ? Response.Success()
                : Response.Failure(new InvariantException("Un libellé de ressource ne doit contenir que des lettres minuscule, chiffres ou tirets ('-') !"));

        #endregion
        #region DOMAIN EVENTS

            public IEnumerable<IDomainEvent> DomainEvents { get; internal init; } = [];
            public Resource WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
                domainEvents = this.DomainEvents;
                return this with { DomainEvents = [] };
            }

        #endregion
    #endregion
    
}