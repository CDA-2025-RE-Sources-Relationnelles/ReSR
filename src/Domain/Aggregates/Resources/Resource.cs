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

        /// <summary> The resource's description. </summary>
        public string Description { get; internal init; } = null!;

        /// <summary> The resource's category </summary>
        public Category Category { get; internal set; } = null!;

        /// <summary> The resource's targeted relationships. </summary>
        public Relationships Relationships { get; internal init; }

        /// <summary> The resource's visibility. </summary>
        public Visibility Visibility { get; internal init; } = Visibility.Private;


        /// <summary> The instant at which the resource was published. </summary>
        public DateTime PublishedAt { get; internal init; } = DateTime.UtcNow;

        /// <summary> The instant at which the resource was last edited. </summary>
        public DateTime EditedAt { get; internal init; } = DateTime.UtcNow;



        /// <summary> The resource's owner, if any. </summary>
        public User? Owner { get; internal init; }

        /// <summary> The users that liked this resource. </summary>
        public virtual ICollection<User> LikedBy { get; internal set; } = [];

        /// <summary> The users that bookmarked this resource. </summary>
        public virtual ICollection<User> BookmarkedBy { get; internal set; } = [];

        /// <summary> The users that exploited this resource. </summary>
        public virtual ICollection<User> ExploitedBy { get; internal set; } = [];

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

            /// <returns> A copy of the resource with a new description. </returns>
            public virtual Resource WithDescription(string value) =>
                this with {
                    EditedAt    = DateTime.UtcNow,
                    Description = value,
                };

            /// <returns> A copy of the resource with a new category. </returns>
            public virtual Resource WithCategory(Category value) {
                this.Category = value;
                return this with {
                    EditedAt = DateTime.UtcNow,
                };
            }

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
            public virtual Resource WithLikeFrom(User from, bool value) {
                if(value) {
                    if (!this.LikedBy.Any(x => x.Id == from.Id))
                        this.LikedBy = [.. this.LikedBy, from];
                } else {
                    List<User> likedBy = [.. this.LikedBy];
                    likedBy.Remove(from);
                    this.LikedBy = likedBy;
                }

                return this;
            }

            /// <returns> A copy of the resource with a bookmark set or unset from the given user. </returns>
            public virtual Resource WithBookmarkFrom(User from, bool value) {
                if(value) {
                    if (!this.BookmarkedBy.Any(x => x.Id == from.Id))
                        this.BookmarkedBy = [.. this.BookmarkedBy, from];
                } else {
                    List<User> bookmarkedBy = [.. this.BookmarkedBy];
                    bookmarkedBy.Remove(from);
                    this.BookmarkedBy = bookmarkedBy;
                }

                return this;
            }

            /// <returns>
            /// A copy of the resource with an exploit set or unset from the given user.
            /// If the user had bookmarked the resource, it will be removed from their bookmarked resource list.
            /// </returns>
            public virtual Resource WithExploitFrom(User from, bool value) {
                if(value) {
                    if (!this.ExploitedBy.Any(x => x.Id == from.Id))
                        this.ExploitedBy = [.. this.ExploitedBy, from];
                } else {
                    List<User> exploitedBy = [.. this.ExploitedBy];
                    exploitedBy.Remove(from);
                    this.ExploitedBy = exploitedBy;
                }

                return this;
            }



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



            public virtual IResponse<Resource> TryWithConfirmedVerification() =>
                this.Visibility == Visibility.WaitingForVerification
                    ? Response.Success(this with {
                        Visibility    = Visibility.Public,
                        DomainEvents  = [..this.DomainEvents, new ResourceVerified(this.Id)]
                    }) : Response.Failure<Resource>(new InvariantException($"La ressource n'est pas à vérifier !"));

            /// <returns> A copy of the resource with rejected verification if being verified. </returns>
            public virtual IResponse<Resource> TryWithRejectedVerification() =>
                this.Visibility == Visibility.WaitingForVerification
                ? Response.Success(this with {
                    Visibility    = Visibility.Suspended,
                    DomainEvents  = [..this.DomainEvents, new ResourceRejected(this.Id)]
                }) : Response.Failure<Resource>(new InvariantException($"La ressource n'est pas à vérifier !"));

        #endregion
        #region INVARIANTS

            protected static IResponse TryVerifyTitleInvariant(string value) =>
                value.Trim().Length >= 4
                ? Response.Success()
                : Response.Failure(new InvariantException("Un titre de ressource doit contenir au moins 4 caractères !"));

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