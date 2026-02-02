using System.Net.Mail;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Accounts;

/// <summary>
/// An account base record.
/// </summary>
/// <typeparam name="T">The account's type.</typeparam>
public abstract record Account<T>(Id Id = default) : IAggregateRoot<T> where T : Account<T> {

    #region PROPERTIES

        /// <summary> The mail address of this account. </summary>
        public string Email { get; internal init; } = null!;

        /// <summary> The encrypted password of this account. </summary>
        internal Password Password { get; init; }

    #endregion
    #region METHODS

        /// <returns> A copy of the account if the email is valid. </returns>
        public virtual IResponse<T> TryWithMailAddress(string value) =>
            TryVerifyEmailInvariant(value)
                .OnSuccess(() => (T)((this.Email != value)
                    ? this with {
                        DomainEvents = [..this.DomainEvents, new AccountEmailChanged<T>(this.Id, this.Email!, value)],
                        Email        = value
                    } : this));

        /// <returns> A copy of the account if the password is valid. </returns>
        public virtual IResponse<T> TryWithPassword(string value) =>
            TryVerifyPasswordInvariant(value).OnSuccess(() =>
            
                Password
                    .TryCreate(value)
                    .OnSuccess(password => (T)(this with { Password = password }))
                
            );

        /// <returns> A successful response if the password matches the account's. </returns>
        public virtual IResponse TryVerifyPassword(string value) =>
            this.Password.TryVerify(value);

        protected static IResponse TryVerifyPasswordInvariant(string value) =>
            string.Concat(value.Where(char.IsLower)).Length >= 4 &&
            string.Concat(value.Where(char.IsUpper)).Length >= 4 &&
            string.Concat(value.Where(char.IsAsciiDigit)).Length >= 4
            ? Response.Success()
            : Response.Failure<T>(new InvariantException("Un mot de passe doit contenir au moins 4 minuscules, 4 majuscules, et 4 chiffres !"));

        protected static IResponse TryVerifyEmailInvariant(string value) =>
            MailAddress.TryCreate(value, out var _)
            ? Response.Success()
            : Response.Failure<T>(new InvariantException("Une adresse électronique doit avoir le format 'nom@domaine' !"));

        public IEnumerable<IDomainEvent> DomainEvents { get; protected init; } = [];
        public T WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
            domainEvents = this.DomainEvents;
            return (T)(this with { DomainEvents = [] });
        }

    #endregion
    
}