using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Core;
using ReSR.Domain.Extensions;

namespace ReSR.Domain.Aggregates.Accounts;
public record User : Account<User>, IAggregateRoot<User> {

    #region PROPERTIES

        /// <summary> The user's social identifier. </summary>
        public string Username { get; internal init; } = null!;

        /// <summary> The user's moderation permissions. </summary>
        public UserPermissions Permissions { get; internal init; }



        /// <summary> The collection of users liked by this user. </summary>
        public virtual IEnumerable<User> LikedUsers { get; internal init; } = new HashSet<User>();

        /// <summary> The collection of users that liked this user. </summary>
        public virtual IEnumerable<User> LikedBy { get; internal init; } = new HashSet<User>();

        /// <summary> The collection of mutually liked users. </summary>
        public virtual IEnumerable<User> Friends => this.LikedUsers.Union(this.LikedBy);



        /// <summary> The date of the first activity of this user. </summary>
        public DateTime FirstActivity { get; internal init; } = DateTime.UtcNow;

        /// <summary> The date of the last activity of this user. </summary>
        public DateTime LastActivity { get; internal init; } = DateTime.UtcNow;


        /// <summary> The user's bookmarked resources. </summary>
        public virtual ICollection<Resource> Bookmarks     { get; internal init; } = [];

        /// <summary> The user's published resources. </summary>
        public virtual ICollection<Resource> OwnedResources { get; internal init; } = [];

        /// <summary> The user's resource verification assignments. </summary>
        public virtual ICollection<Resource> ResourcesToVerify { get; internal init; } = [];

        /// <summary> Whether or not the user's account has been temporaly deactivated. </summary>
        public bool Suspended { get; internal init; }

        /// <summary> The date at which the user's automatic anonymization process started, if any. </summary>
        public DateTime? AnonymizationProcessStartedAt { get; internal init; }

        /// <summary> Whether or not the user has been anonymized. </summary>
        public virtual bool IsAnonymous => string.IsNullOrWhiteSpace(this.Email);



    #endregion
    #region CONSTRUCTORS

        public static IResponse<User> TryCreate(
            string          username,
            string          email,
            string          password,
            UserPermissions permissions = UserPermissions.None
        ) => TryVerifyUsernameInvariant(username)
                .OnSuccess(() => TryVerifyEmailInvariant(email))
                .OnSuccess(() => Password.TryCreate(password))
                .OnSuccess(password => new User {
                    Username     = username,
                    Email        = email,
                    Password     = password,
                    Permissions  = permissions,
                    DomainEvents = [new AccountCreated<User>(email)]
                });

    #endregion
    #region METHODS
        #region UPDATES

            /// <returns> A copy of the user account with the given permissions. </returns>
            public virtual IResponse<User> TryWithUsername(string value) =>
                TryVerifyUsernameInvariant(value).OnSuccess(() => this with { Username = value});

            /// <returns> A copy of the user account with the given permissions. </returns>
            public virtual User WithPermissions(UserPermissions value) =>
                this with { Permissions = value };



            /// <returns> A copy of the user account with a like frm the given user. </returns>
            public virtual User WithLikeFrom(User from, bool value) =>
                value
                ? this with {
                    LikedBy = [..this.LikedBy, from],
                    DomainEvents = !this.LikedBy.Contains(from)
                        ? [..this.DomainEvents, new UserMutuallyLiked(this.Id, from.Id)]
                        : this.DomainEvents
                } : this with { LikedBy = this.LikedBy.Where(x => x.Id != from.Id) };



            /// <returns> A copy of the user account with a new activity. </returns>
            public virtual User WithNewActivity() =>
                this with { LastActivity = DateTime.UtcNow };



            /// <returns> A copy of the user account with the given suspension. </returns>
            public virtual User WithSuspension(bool value = true) =>
                this with {
                    Suspended    = value,
                    DomainEvents = this.Suspended != value
                        ? [..this.DomainEvents, new UserSuspensionChanged(this.Id, value)]
                        : this.DomainEvents
                };

            /// <returns> A copy of the user account as anonymized. </returns>
            public virtual User AsAnonymized() =>
                this with {
                    Email        = string.Empty,
                    Password     = Password.FromNoise(),
                    DomainEvents = this.Email is not null
                        ? [..this.DomainEvents, new UserAnonymized(this.Id)]
                        : this.DomainEvents
                };

            /// <returns> A copy of the user account with a new anonymization process if none were already started. </returns>
            public virtual IResponse<User> TryWithNewAnonymizationProcess() =>
                this.AnonymizationProcessStartedAt is not null
                    ? Response.Failure<User>(new InvariantException("Le compte est déjà en train d'être anonymisé !"))
                    : Response.Success(this with {
                        AnonymizationProcessStartedAt = DateTime.UtcNow,
                        DomainEvents                  = [..this.DomainEvents, new UserAnonymizationProcessStarted(this.Id)]
                    });

            /// <returns> A successful response if the permissions matches the admin's. </returns>
            public virtual IResponse TryVerifyPermissions(UserPermissions value) =>
                this.Permissions.HasFlag(value)
                ? Response.Success()
                : Response.Failure($"L'utilisateur n'a pas les permissions suivantes '{value.GetUniqueValues()}'");


        #endregion
        #region INVARIANTS

            public static IResponse TryVerifyUsernameInvariant(string value) =>
                value.All(char.IsAsciiLetterOrDigit) && value.Length >= 4
                ? Response.Success()
                : Response.Failure<User>(new InvariantException("Un nom d'utilisateur doit contenir au moins 4 caractères alphanumériques !"));

        #endregion
    #endregion
    
}